# face_recognition_service.py
import cv2
import face_recognition
import numpy as np
import os
import json
import sys
import base64
from datetime import datetime
import pickle
import argparse
import time

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
            with open(config_path, 'w', encoding='utf-8') as f:
                json.dump(default_config, f, indent=4, ensure_ascii=False)
            return default_config
    
    def load_known_faces(self):
        """Load tất cả khuôn mặt đã đăng ký từ thư mục faces"""
        self.known_face_encodings = []
        self.known_face_names = []
        self.known_employee_ids = []
        
        for filename in os.listdir(self.faces_folder):
            if filename.endswith(('.jpg', '.jpeg', '.png')):
                try:
                    name_part = filename.split('.')[0]
                    employee_id = name_part.split('_')[0]
                    employee_name = '_'.join(name_part.split('_')[1:]).replace('_', ' ')
                    
                    image_path = os.path.join(self.faces_folder, filename)
                    image = face_recognition.load_image_file(image_path)
                    
                    face_encodings = face_recognition.face_encodings(image)
                    if len(face_encodings) > 0:
                        self.known_face_encodings.append(face_encodings[0])
                        self.known_face_names.append(employee_name)
                        self.known_employee_ids.append(employee_id)
                        
                except Exception as e:
                    print(f"Error loading {filename}: {str(e)}")
        
        print(f"Loaded {len(self.known_face_encodings)} known faces")
    
    def register_face(self, employee_id, employee_name, image_path_or_base64):
        """Đăng ký khuôn mặt mới"""
        try:
            # Xử lý input
            if isinstance(image_path_or_base64, str) and image_path_or_base64.startswith('data:image'):
                # Base64 image
                image_data = image_path_or_base64.split(',')[1]
                image_bytes = base64.b64decode(image_data)
                
                temp_path = f"temp_{employee_id}_{int(time.time())}.jpg"
                with open(temp_path, 'wb') as f:
                    f.write(image_bytes)
                
                image = face_recognition.load_image_file(temp_path)
                os.remove(temp_path)
            else:
                # File path
                image = face_recognition.load_image_file(image_path_or_base64)
            
            # Tìm encoding khuôn mặt
            face_encodings = face_recognition.face_encodings(image)
            
            if len(face_encodings) == 0:
                return {"success": False, "message": "Không tìm thấy khuôn mặt trong ảnh"}
            
            if len(face_encodings) > 1:
                return {"success": False, "message": "Phát hiện nhiều khuôn mặt, vui lòng chọn ảnh có 1 khuôn mặt"}
            
            # Kiểm tra khuôn mặt đã tồn tại chưa
            face_distances = face_recognition.face_distance(self.known_face_encodings, face_encodings[0])
            if len(face_distances) > 0 and min(face_distances) < self.tolerance:
                existing_index = np.argmin(face_distances)
                existing_id = self.known_employee_ids[existing_index]
                return {
                    "success": False, 
                    "message": f"Khuôn mặt đã được đăng ký cho nhân viên ID: {existing_id}"
                }
            
            # Lưu ảnh khuôn mặt
            face_filename = f"{employee_id}_{employee_name.replace(' ', '_')}.jpg"
            face_path = os.path.join(self.faces_folder, face_filename)
            
            # Chuyển từ RGB sang BGR cho OpenCV
            image_bgr = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
            cv2.imwrite(face_path, image_bgr)
            
            # Reload known faces
            self.load_known_faces()
            
            return {
                "success": True, 
                "message": "Đăng ký khuôn mặt thành công",
                "face_path": face_path
            }
            
        except Exception as e:
            return {"success": False, "message": f"Lỗi: {str(e)}"}
    
    def recognize_face_from_camera(self, timeout=30):
        """Nhận diện khuôn mặt từ camera"""
        cap = cv2.VideoCapture(self.config.get("camera_index", 0))
        
        if not cap.isOpened():
            return {"success": False, "message": "Không thể mở camera"}
        
        # Cài đặt độ phân giải camera
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, self.config.get("frame_width", 640))
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, self.config.get("frame_height", 480))
        
        start_time = time.time()
        process_this_frame = True
        
        try:
            while True:
                ret, frame = cap.read()
                if not ret:
                    break
                
                # Kiểm tra timeout
                if time.time() - start_time > timeout:
                    break
                
                # Xử lý mỗi frame thứ 2 để tăng tốc độ
                if process_this_frame:
                    # Resize frame để xử lý nhanh hơn
                    small_frame = cv2.resize(frame, (0, 0), fx=0.25, fy=0.25)
                    rgb_small_frame = small_frame[:, :, ::-1]
                    
                    # Tìm khuôn mặt trong frame
                    face_locations = face_recognition.face_locations(rgb_small_frame)
                    face_encodings = face_recognition.face_encodings(rgb_small_frame, face_locations)
                    
                    for face_encoding in face_encodings:
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
                                    if not os.path.exists(attendance_folder):
                                        os.makedirs(attendance_folder)
                                    
                                    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
                                    attendance_image_path = os.path.join(
                                        attendance_folder, 
                                        f"{employee_id}_{timestamp}.jpg"
                                    )
                                    cv2.imwrite(attendance_image_path, frame)
                                
                                cap.release()
                                return {
                                    "success": True,
                                    "employee_id": employee_id,
                                    "employee_name": employee_name,
                                    "confidence": round(confidence * 100, 2),
                                    "timestamp": datetime.now().isoformat(),
                                    "attendance_image": attendance_image_path
                                }
                
                process_this_frame = not process_this_frame
                
                # Hiển thị preview (optional)
                cv2.imshow('Face Recognition - Press ESC to exit', frame)
                if cv2.waitKey(1) & 0xFF == 27:  # ESC key
                    break
            
            cap.release()
            cv2.destroyAllWindows()
            return {"success": False, "message": "Không nhận diện được khuôn mặt"}
            
        except Exception as e:
            cap.release()
            cv2.destroyAllWindows()
            return {"success": False, "message": f"Lỗi: {str(e)}"}
    
    def recognize_face_from_image(self, image_path_or_base64):
        """Nhận diện khuôn mặt từ ảnh"""
        try:
            # Xử lý input
            if isinstance(image_path_or_base64, str) and image_path_or_base64.startswith('data:image'):
                # Base64 image
                image_data = image_path_or_base64.split(',')[1]
                image_bytes = base64.b64decode(image_data)
                
                temp_path = f"temp_recognize_{int(time.time())}.jpg"
                with open(temp_path, 'wb') as f:
                    f.write(image_bytes)
                
                image = face_recognition.load_image_file(temp_path)
                os.remove(temp_path)
            else:
                # File path
                image = face_recognition.load_image_file(image_path_or_base64)
            
            # Tìm khuôn mặt
            face_locations = face_recognition.face_locations(image)
            face_encodings = face_recognition.face_encodings(image, face_locations)
            
            if len(face_encodings) == 0:
                return {"success": False, "message": "Không tìm thấy khuôn mặt trong ảnh"}
            
            # Nhận diện khuôn mặt đầu tiên
            face_encoding = face_encodings[0]
            face_distances = face_recognition.face_distance(self.known_face_encodings, face_encoding)
            
            if len(face_distances) == 0:
                return {"success": False, "message": "Chưa có khuôn mặt nào được đăng ký"}
            
            best_match_index = np.argmin(face_distances)
            confidence = 1 - face_distances[best_match_index]
            
            if face_distances[best_match_index] < self.tolerance and confidence > self.confidence_threshold:
                employee_id = self.known_employee_ids[best_match_index]
                employee_name = self.known_face_names[best_match_index]
                
                return {
                    "success": True,
                    "employee_id": employee_id,
                    "employee_name": employee_name,
                    "confidence": round(confidence * 100, 2),
                    "timestamp": datetime.now().isoformat()
                }
            else:
                return {
                    "success": False, 
                    "message": f"Khuôn mặt không được nhận diện (độ tin cậy: {round(confidence * 100, 2)}%)"
                }
                
        except Exception as e:
            return {"success": False, "message": f"Lỗi: {str(e)}"}
    
    def delete_registered_face(self, employee_id):
        """Xóa khuôn mặt đã đăng ký"""
        try:
            deleted = False
            for filename in os.listdir(self.faces_folder):
                if filename.startswith(f"{employee_id}_"):
                    file_path = os.path.join(self.faces_folder, filename)
                    os.remove(file_path)
                    deleted = True
            
            if deleted:
                self.load_known_faces()
                return {"success": True, "message": "Xóa khuôn mặt thành công"}
            else:
                return {"success": False, "message": "Không tìm thấy khuôn mặt để xóa"}
                
        except Exception as e:
            return {"success": False, "message": f"Lỗi: {str(e)}"}
    
    def get_registered_faces(self):
        """Lấy danh sách khuôn mặt đã đăng ký"""
        faces = []
        for i in range(len(self.known_employee_ids)):
            faces.append({
                "employee_id": self.known_employee_ids[i],
                "employee_name": self.known_face_names[i]
            })
        return {"success": True, "faces": faces}

def main():
    """Main function để gọi từ command line"""
    parser = argparse.ArgumentParser(description='Face Recognition Service')
    parser.add_argument('action', choices=['register', 'recognize_camera', 'recognize_image', 'delete', 'list'])
    parser.add_argument('--employee_id', help='Employee ID')
    parser.add_argument('--employee_name', help='Employee Name')
    parser.add_argument('--image_path', help='Path to image file')
    parser.add_argument('--timeout', type=int, default=30, help='Recognition timeout in seconds')
    
    args = parser.parse_args()
    
    service = FaceRecognitionService()
    
    if args.action == 'register':
        if not args.employee_id or not args.employee_name or not args.image_path:
            print(json.dumps({"success": False, "message": "Missing required parameters"}))
            return
        
        result = service.register_face(args.employee_id, args.employee_name, args.image_path)
        print(json.dumps(result, ensure_ascii=False))
    
    elif args.action == 'recognize_camera':
        result = service.recognize_face_from_camera(args.timeout)
        print(json.dumps(result, ensure_ascii=False))
    
    elif args.action == 'recognize_image':
        if not args.image_path:
            print(json.dumps({"success": False, "message": "Missing image_path parameter"}))
            return
        
        result = service.recognize_face_from_image(args.image_path)
        print(json.dumps(result, ensure_ascii=False))
    
    elif args.action == 'delete':
        if not args.employee_id:
            print(json.dumps({"success": False, "message": "Missing employee_id parameter"}))
            return
        
        result = service.delete_registered_face(args.employee_id)
        print(json.dumps(result, ensure_ascii=False))
    
    elif args.action == 'list':
        result = service.get_registered_faces()
        print(json.dumps(result, ensure_ascii=False))

if __name__ == "__main__":
    main()