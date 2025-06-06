#!/usr/bin/env python
# -*- coding: utf-8 -*-
import sys
import os
import locale
import signal
import threading
from concurrent.futures import ThreadPoolExecutor, TimeoutError as FuturesTimeoutError

# Set encoding cho Windows
if sys.platform.startswith('win'):
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')

# Set locale
try:
    locale.setlocale(locale.LC_ALL, 'en_US.UTF-8')
except:
    try:
        locale.setlocale(locale.LC_ALL, 'C.UTF-8')
    except:
        pass

import cv2
import face_recognition
import numpy as np
import json
import base64
from datetime import datetime
import argparse
import time
import unicodedata
import re

class TimeoutException(Exception):
    pass

class FaceRecognitionService:
    def __init__(self, config_path="config.json"):
        self.config = self.load_config(config_path)
        self.faces_folder = self.config.get("faces_folder", "faces")
        self.tolerance = self.config.get("tolerance", 0.6)
        self.confidence_threshold = self.config.get("confidence_threshold", 0.4)
        
        self.known_face_encodings = []
        self.known_face_names = []
        self.known_employee_ids = []
        
        # Tạo thư mục faces nếu chưa có
        if not os.path.exists(self.faces_folder):
            os.makedirs(self.faces_folder)
            
        self.load_known_faces()
    
    def load_config(self, config_path):
        """Load cấu hình từ file JSON"""
        try:
            with open(config_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except FileNotFoundError:
            # Tạo config mặc định nếu file không tồn tại
            default_config = {
                "faces_folder": "faces",
                "tolerance": 0.6,
                "confidence_threshold": 0.4,
                "camera_index": 0,
                "frame_width": 640,
                "frame_height": 480,
                "recognition_timeout": 30,
                "save_attendance_images": True,
                "attendance_images_folder": "attendance_images"
            }
            try:
                with open(config_path, 'w', encoding='utf-8') as f:
                    json.dump(default_config, f, indent=4, ensure_ascii=False)
            except:
                pass  # Ignore if can't create config file
            return default_config
    
    def load_known_faces(self):
        """Load tất cả khuôn mặt đã đăng ký từ thư mục faces"""
        self.known_face_encodings = []
        self.known_face_names = []
        self.known_employee_ids = []
        
        if not os.path.exists(self.faces_folder):
            return
        
        for filename in os.listdir(self.faces_folder):
            if filename.lower().endswith(('.jpg', '.jpeg', '.png', '.bmp')):
                try:
                    name_part = filename.split('.')[0]
                    parts = name_part.split('_', 1)
                    
                    if len(parts) >= 2:
                        employee_id = parts[0]
                        employee_name = parts[1].replace('_', ' ')
                    else:
                        employee_id = name_part
                        employee_name = name_part
                    
                    image_path = os.path.join(self.faces_folder, filename)
                    
                    # Load image with error handling
                    try:
                        image = face_recognition.load_image_file(image_path)
                    except Exception as e:
                        print(f"Error loading image {filename}: {str(e)}", file=sys.stderr)
                        continue
                    
                    face_encodings = face_recognition.face_encodings(image)
                    if len(face_encodings) > 0:
                        self.known_face_encodings.append(face_encodings[0])
                        self.known_face_names.append(employee_name)
                        self.known_employee_ids.append(employee_id)
                        
                except Exception as e:
                    print(f"Error processing {filename}: {str(e)}", file=sys.stderr)
                    continue
        
        print(f"Loaded {len(self.known_face_encodings)} known faces", file=sys.stderr)
    
    def sanitize_filename(self, filename):
        """Tạo tên file an toàn từ tên tiếng Việt"""
        # Normalize unicode
        filename = unicodedata.normalize('NFD', filename)
        
        # Loại bỏ dấu
        filename = ''.join(char for char in filename if unicodedata.category(char) != 'Mn')
        
        # Thay thế ký tự đặc biệt
        replacements = {
            'đ': 'd', 'Đ': 'D',
            ' ': '_',
            'ă': 'a', 'â': 'a', 'á': 'a', 'à': 'a', 'ả': 'a', 'ã': 'a', 'ạ': 'a',
            'ê': 'e', 'é': 'e', 'è': 'e', 'ẻ': 'e', 'ẽ': 'e', 'ẹ': 'e',
            'ô': 'o', 'ơ': 'o', 'ó': 'o', 'ò': 'o', 'ỏ': 'o', 'õ': 'o', 'ọ': 'o',
            'ư': 'u', 'ú': 'u', 'ù': 'u', 'ủ': 'u', 'ũ': 'u', 'ụ': 'u',
            'í': 'i', 'ì': 'i', 'ỉ': 'i', 'ĩ': 'i', 'ị': 'i',
            'ý': 'y', 'ỳ': 'y', 'ỷ': 'y', 'ỹ': 'y', 'ỵ': 'y'
        }
        
        for viet, eng in replacements.items():
            filename = filename.replace(viet, eng)
            filename = filename.replace(viet.upper(), eng.upper())
        
        # Chỉ giữ lại chữ cái, số và dấu gạch dưới
        filename = re.sub(r'[^a-zA-Z0-9_]', '', filename)
        
        return filename[:50]  # Giới hạn độ dài

    def register_face_with_timeout(self, employee_id, employee_name, image_path, timeout_seconds=60):
        """Đăng ký khuôn mặt với timeout"""
        def timeout_handler(signum, frame):
            raise TimeoutException("Registration timeout")
        
        # Set timeout signal (Unix only)
        if hasattr(signal, 'SIGALRM'):
            signal.signal(signal.SIGALRM, timeout_handler)
            signal.alarm(timeout_seconds)
        
        try:
            result = self.register_face(employee_id, employee_name, image_path)
            return result
        except TimeoutException:
            return {"success": False, "message": "Đăng ký hết thời gian chờ"}
        finally:
            if hasattr(signal, 'SIGALRM'):
                signal.alarm(0)  # Cancel timeout

    def register_face(self, employee_id, employee_name, image_path_or_base64):
        """Đăng ký khuôn mặt mới"""
        try:
            # Sanitize tên nhân viên để tránh lỗi encoding
            safe_employee_name = self.sanitize_filename(employee_name)
            
            # Xử lý input image
            if isinstance(image_path_or_base64, str) and image_path_or_base64.startswith('data:image'):
                # Base64 image
                try:
                    image_data = image_path_or_base64.split(',')[1]
                    image_bytes = base64.b64decode(image_data)
                    
                    temp_path = f"temp_{employee_id}_{int(time.time())}.jpg"
                    with open(temp_path, 'wb') as f:
                        f.write(image_bytes)
                    
                    image = face_recognition.load_image_file(temp_path)
                    
                    # Clean up temp file
                    try:
                        os.remove(temp_path)
                    except:
                        pass
                        
                except Exception as e:
                    return {"success": False, "message": f"Lỗi xử lý ảnh base64: {str(e)}"}
            else:
                # File path - đảm bảo encoding đúng
                if not os.path.exists(image_path_or_base64):
                    return {"success": False, "message": f"File không tồn tại: {image_path_or_base64}"}
                
                try:
                    image = face_recognition.load_image_file(image_path_or_base64)
                except Exception as e:
                    return {"success": False, "message": f"Không thể đọc file ảnh: {str(e)}"}
            
            # Tìm encoding khuôn mặt
            try:
                face_encodings = face_recognition.face_encodings(image)
            except Exception as e:
                return {"success": False, "message": f"Lỗi phân tích khuôn mặt: {str(e)}"}
            
            if len(face_encodings) == 0:
                return {"success": False, "message": "Không tìm thấy khuôn mặt trong ảnh"}
            
            if len(face_encodings) > 1:
                return {"success": False, "message": "Phát hiện nhiều khuôn mặt, vui lòng chọn ảnh có 1 khuôn mặt"}
            
            # Kiểm tra trùng lặp
            if len(self.known_face_encodings) > 0:
                face_distances = face_recognition.face_distance(self.known_face_encodings, face_encodings[0])
                min_distance = np.min(face_distances)
                
                if min_distance < self.tolerance:
                    min_index = np.argmin(face_distances)
                    existing_employee = self.known_employee_ids[min_index]
                    
                    if existing_employee != employee_id:
                        return {
                            "success": False, 
                            "message": f"Khuôn mặt đã được đăng ký cho nhân viên khác: {existing_employee}"
                        }
            
            # Lưu ảnh khuôn mặt với tên file an toàn
            face_filename = f"{employee_id}_{safe_employee_name}.jpg"
            face_path = os.path.join(self.faces_folder, face_filename)
            
            try:
                # Chuyển từ RGB sang BGR cho OpenCV
                image_bgr = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
                
                # Ensure faces folder exists
                os.makedirs(self.faces_folder, exist_ok=True)
                
                success = cv2.imwrite(face_path, image_bgr)
                if not success:
                    return {"success": False, "message": "Không thể lưu file ảnh"}
                    
            except Exception as e:
                return {"success": False, "message": f"Lỗi lưu ảnh: {str(e)}"}
            
            # Reload known faces
            try:
                self.load_known_faces()
            except Exception as e:
                return {"success": False, "message": f"Lỗi tải lại dữ liệu: {str(e)}"}
            
            return {
                "success": True, 
                "message": "Đăng ký khuôn mặt thành công",
                "face_path": face_path
            }
            
        except Exception as e:
            return {"success": False, "message": f"Lỗi hệ thống: {str(e)}"}

    def recognize_face_from_camera(self, timeout=30):
        """Nhận diện khuôn mặt từ camera với timeout cải tiến"""
        cap = None
        try:
            cap = cv2.VideoCapture(self.config.get("camera_index", 0))
            
            if not cap.isOpened():
                return {"success": False, "message": "Không thể mở camera"}
            
            # Cài đặt độ phân giải camera
            cap.set(cv2.CAP_PROP_FRAME_WIDTH, self.config.get("frame_width", 640))
            cap.set(cv2.CAP_PROP_FRAME_HEIGHT, self.config.get("frame_height", 480))
            cap.set(cv2.CAP_PROP_FPS, 30)
            
            start_time = time.time()
            process_this_frame = True
            frame_count = 0
            
            while True:
                ret, frame = cap.read()
                if not ret:
                    time.sleep(0.1)
                    continue
                
                frame_count += 1
                
                # Kiểm tra timeout
                elapsed_time = time.time() - start_time
                if elapsed_time > timeout:
                    break
                
                # Xử lý mỗi frame thứ 3 để tăng tốc độ
                if frame_count % 3 != 0:
                    continue
                
                if process_this_frame:
                    # Resize frame để xử lý nhanh hơn
                    small_frame = cv2.resize(frame, (0, 0), fx=0.25, fy=0.25)
                    rgb_small_frame = small_frame[:, :, ::-1]
                    
                    try:
                        # Tìm khuôn mặt trong frame
                        face_locations = face_recognition.face_locations(rgb_small_frame, model="hog")
                        if len(face_locations) == 0:
                            process_this_frame = not process_this_frame
                            continue
                            
                        face_encodings = face_recognition.face_encodings(rgb_small_frame, face_locations)
                        
                        for face_encoding in face_encodings:
                            if len(self.known_face_encodings) == 0:
                                continue
                                
                            # So sánh với khuôn mặt đã biết
                            face_distances = face_recognition.face_distance(self.known_face_encodings, face_encoding)
                            
                            if len(face_distances) > 0:
                                best_match_index = np.argmin(face_distances)
                                confidence = 1 - face_distances[best_match_index]
                                
                                if face_distances[best_match_index] < self.tolerance and confidence > self.confidence_threshold:
                                    employee_id = self.known_employee_ids[best_match_index]
                                    employee_name = self.known_face_names[best_match_index]
                                    
                                    # Lưu ảnh chấm công nếu được cấu hình
                                    attendance_image_path = None
                                    if self.config.get("save_attendance_images", True):
                                        attendance_folder = self.config.get("attendance_images_folder", "attendance_images")
                                        os.makedirs(attendance_folder, exist_ok=True)
                                        
                                        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
                                        attendance_image_path = os.path.join(
                                            attendance_folder, 
                                            f"{employee_id}_{timestamp}.jpg"
                                        )
                                        try:
                                            cv2.imwrite(attendance_image_path, frame)
                                        except:
                                            attendance_image_path = None
                                    
                                    return {
                                        "success": True,
                                        "employee_id": employee_id,
                                        "employee_name": employee_name,
                                        "confidence": round(confidence * 100, 2),
                                        "timestamp": datetime.now().isoformat(),
                                        "attendance_image": attendance_image_path
                                    }
                    except Exception as e:
                        print(f"Face recognition error: {str(e)}", file=sys.stderr)
                        continue
                
                process_this_frame = not process_this_frame
                
                # Kiểm tra key press để thoát sớm (optional)
                if cv2.waitKey(1) & 0xFF == 27:  # ESC key
                    break
            
            return {"success": False, "message": "Không nhận diện được khuôn mặt trong thời gian cho phép"}
            
        except Exception as e:
            return {"success": False, "message": f"Lỗi hệ thống: {str(e)}"}
        finally:
            if cap is not None:
                cap.release()
            cv2.destroyAllWindows()
    
    def delete_registered_face(self, employee_id):
        """Xóa khuôn mặt đã đăng ký"""
        try:
            deleted = False
            files_to_delete = []
            
            if not os.path.exists(self.faces_folder):
                return {"success": False, "message": "Thư mục faces không tồn tại"}
            
            # Tìm tất cả file có employee_id
            for filename in os.listdir(self.faces_folder):
                if filename.startswith(f"{employee_id}_"):
                    files_to_delete.append(filename)
            
            # Xóa các file
            for filename in files_to_delete:
                try:
                    file_path = os.path.join(self.faces_folder, filename)
                    os.remove(file_path)
                    deleted = True
                except Exception as e:
                    print(f"Error deleting {filename}: {str(e)}", file=sys.stderr)
            
            if deleted:
                self.load_known_faces()
                return {"success": True, "message": "Xóa khuôn mặt thành công"}
            else:
                return {"success": False, "message": "Không tìm thấy khuôn mặt để xóa"}
                
        except Exception as e:
            return {"success": False, "message": f"Lỗi hệ thống: {str(e)}"}
    
    def get_registered_faces(self):
        """Lấy danh sách khuôn mặt đã đăng ký"""
        try:
            faces = []
            for i in range(len(self.known_employee_ids)):
                faces.append({
                    "employee_id": self.known_employee_ids[i],
                    "employee_name": self.known_face_names[i]
                })
            return {"success": True, "faces": faces}
        except Exception as e:
            return {"success": False, "message": f"Lỗi hệ thống: {str(e)}", "faces": []}

def main():
    """Main function để gọi từ command line"""
    try:
        parser = argparse.ArgumentParser(description='Face Recognition Service')
        parser.add_argument('action', choices=['register', 'recognize_camera', 'recognize_image', 'delete', 'list'])
        parser.add_argument('--employee_id', help='Employee ID')
        parser.add_argument('--employee_name', help='Employee Name')
        parser.add_argument('--image_path', help='Path to image file')
        parser.add_argument('--timeout', type=int, default=30, help='Recognition timeout in seconds')
        
        args = parser.parse_args()
        
        service = FaceRecognitionService()
        result = None
        
        if args.action == 'register':
            if not args.employee_id or not args.employee_name or not args.image_path:
                result = {"success": False, "message": "Thiếu tham số bắt buộc: employee_id, employee_name, image_path"}
            else:
                # Use timeout wrapper for registration
                result = service.register_face_with_timeout(args.employee_id, args.employee_name, args.image_path, 60)
                
        elif args.action == 'recognize_camera':
            result = service.recognize_face_from_camera(args.timeout)
            
        elif args.action == 'delete':
            if not args.employee_id:
                result = {"success": False, "message": "Thiếu tham số employee_id"}
            else:
                result = service.delete_registered_face(args.employee_id)
                
        elif args.action == 'list':
            result = service.get_registered_faces()
            
        else:
            result = {"success": False, "message": f"Hành động không được hỗ trợ: {args.action}"}
        
        # Output với encoding an toàn
        if result:
            output = json.dumps(result, ensure_ascii=False, separators=(',', ':'))
            print(output)
        else:
            print(json.dumps({"success": False, "message": "Không có kết quả"}, ensure_ascii=False))
        
    except KeyboardInterrupt:
        print(json.dumps({"success": False, "message": "Đã hủy bởi người dùng"}, ensure_ascii=False))
    except Exception as e:
        error_result = {
            "success": False, 
            "message": f"Lỗi hệ thống: {str(e)}"
        }
        print(json.dumps(error_result, ensure_ascii=False))

if __name__ == "__main__":
    main()