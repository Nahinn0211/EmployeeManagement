"""
Hệ thống nhận diện khuôn mặt cho chấm công
Author: Employee Management System
Version: 2.0
Requirements: opencv-python, face-recognition, numpy, pillow
"""

import cv2
import face_recognition
import pickle
import numpy as np
import os
import sys
import json
from datetime import datetime
import threading
import time
import argparse

class FaceRecognitionSystem:
    def __init__(self, encodings_file='face_encodings.pkl', confidence_threshold=0.6):
        self.encodings_file = encodings_file
        self.confidence_threshold = confidence_threshold
        self.known_face_encodings = []
        self.known_face_names = []
        self.is_camera_running = False
        self.capture_thread = None
        self.load_known_faces()
        
    def load_known_faces(self):
        """Tải dữ liệu khuôn mặt đã đăng ký"""
        if os.path.exists(self.encodings_file):
            try:
                with open(self.encodings_file, 'rb') as f:
                    data = pickle.load(f)
                    self.known_face_encodings = data.get('encodings', [])
                    self.known_face_names = data.get('names', [])
                print(f"Loaded {len(self.known_face_names)} known faces")
            except Exception as e:
                print(f"Error loading face encodings: {e}")
                self.known_face_encodings = []
                self.known_face_names = []
        else:
            print("No face encodings file found. Starting with empty database.")
    
    def save_known_faces(self):
        """Lưu dữ liệu khuôn mặt"""
        try:
            data = {
                'encodings': self.known_face_encodings,
                'names': self.known_face_names,
                'timestamp': datetime.now().isoformat()
            }
            with open(self.encodings_file, 'wb') as f:
                pickle.dump(data, f)
            print("Face encodings saved successfully")
            return True
        except Exception as e:
            print(f"Error saving face encodings: {e}")
            return False
    
    def register_face(self, employee_id, num_samples=15, camera_index=0):
        """Đăng ký khuôn mặt cho nhân viên"""
        print(f"Starting face registration for employee {employee_id}")
        print(f"Will collect {num_samples} samples")
        
        cap = cv2.VideoCapture(camera_index)
        if not cap.isOpened():
            print("Error: Could not open camera")
            return False
        
        # Cài đặt camera
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
        cap.set(cv2.CAP_PROP_FPS, 30)
        
        face_encodings = []
        samples_collected = 0
        consecutive_no_face = 0
        max_no_face = 30
        
        print("Position your face in the camera frame...")
        print("Registration will start automatically when face is detected")
        
        collection_started = False
        start_delay = 0
        
        while samples_collected < num_samples:
            ret, frame = cap.read()
            if not ret:
                print("Error reading frame")
                break
            
            # Lật frame theo chiều ngang
            frame = cv2.flip(frame, 1)
            
            # Chuyển sang RGB
            rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            
            # Tìm khuôn mặt
            face_locations = face_recognition.face_locations(rgb_frame, model="hog")
            current_face_encodings = face_recognition.face_encodings(rgb_frame, face_locations)
            
            # Vẽ khung khuôn mặt và thông tin
            for (top, right, bottom, left) in face_locations:
                cv2.rectangle(frame, (left, top), (right, bottom), (0, 255, 0), 2)
                
                # Tự động bắt đầu thu thập sau 2 giây
                if not collection_started:
                    start_delay += 1
                    if start_delay > 60:  # 2 seconds at 30fps
                        collection_started = True
                        print("Starting automatic face collection...")
                
                if collection_started and len(current_face_encodings) > 0:
                    # Thu thập mẫu
                    face_encodings.extend(current_face_encodings)
                    samples_collected += len(current_face_encodings)
                    consecutive_no_face = 0
                    
                    # Hiển thị tiến trình
                    progress = min(samples_collected, num_samples)
                    cv2.putText(frame, f'Collecting: {progress}/{num_samples}', 
                              (left, top - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)
                    
                    print(f"Sample {progress}/{num_samples} collected")
                    
                    # Delay between samples
                    time.sleep(0.2)
                    
                    if samples_collected >= num_samples:
                        break
            
            if len(face_locations) == 0:
                consecutive_no_face += 1
                cv2.putText(frame, 'Please position your face in frame', (20, 50), 
                          cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 0, 255), 2)
                
                if collection_started and consecutive_no_face > max_no_face:
                    print("Lost face detection. Stopping collection.")
                    break
            else:
                consecutive_no_face = 0
            
            # Hiển thị hướng dẫn
            if not collection_started:
                cv2.putText(frame, "Preparing to capture...", (20, 30), 
                          cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 255, 0), 2)
                cv2.putText(frame, "Look straight at camera", (20, 60), 
                          cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 255, 0), 2)
            
            # Hiển thị thanh tiến trình
            if collection_started:
                progress_width = int((samples_collected / num_samples) * 400)
                cv2.rectangle(frame, (20, frame.shape[0] - 50), (420, frame.shape[0] - 30), (100, 100, 100), -1)
                cv2.rectangle(frame, (20, frame.shape[0] - 50), (20 + progress_width, frame.shape[0] - 30), (0, 255, 0), -1)
                cv2.putText(frame, f'{samples_collected}/{num_samples}', (430, frame.shape[0] - 35), 
                          cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)
            
            cv2.imshow('Face Registration - Press ESC to cancel', frame)
            
            key = cv2.waitKey(1) & 0xFF
            if key == 27:  # ESC
                print("Registration cancelled by user")
                break
        
        cap.release()
        cv2.destroyAllWindows()
        
        if len(face_encodings) < 5:
            print(f"Insufficient face samples collected: {len(face_encodings)}")
            return False
        
        # Tính encoding trung bình
        print(f"Processing {len(face_encodings)} face samples...")
        average_encoding = np.mean(face_encodings, axis=0)
        
        # Kiểm tra chất lượng encoding
        if self._validate_encoding_quality(face_encodings, average_encoding):
            # Thêm vào cơ sở dữ liệu
            employee_id_str = str(employee_id)
            
            # Kiểm tra xem nhân viên đã có trong cơ sở dữ liệu chưa
            if employee_id_str in self.known_face_names:
                # Cập nhật encoding hiện có
                index = self.known_face_names.index(employee_id_str)
                self.known_face_encodings[index] = average_encoding
                print(f"Updated face encoding for employee {employee_id}")
            else:
                # Thêm encoding mới
                self.known_face_encodings.append(average_encoding)
                self.known_face_names.append(employee_id_str)
                print(f"Added new face encoding for employee {employee_id}")
            
            # Lưu vào file
            if self.save_known_faces():
                print("Face registration completed successfully!")
                return True
            else:
                print("Failed to save face encodings")
                return False
        else:
            print("Face encoding quality validation failed")
            return False
    
    def _validate_encoding_quality(self, face_encodings, average_encoding):
        """Kiểm tra chất lượng encoding"""
        if len(face_encodings) < 3:
            return False
        
        # Tính độ biến thiên của các encoding
        distances = [face_recognition.face_distance([average_encoding], encoding)[0] 
                    for encoding in face_encodings]
        
        avg_distance = np.mean(distances)
        max_distance = np.max(distances)
        
        print(f"Encoding quality - Avg distance: {avg_distance:.3f}, Max distance: {max_distance:.3f}")
        
        # Kiểm tra ngưỡng chất lượng
        if avg_distance > 0.4 or max_distance > 0.8:
            print("Warning: High variance in face encodings. Quality may be poor.")
            return False
        
        return True
    
    def recognize_face_from_image(self, image_path):
        """Nhận diện khuôn mặt từ file ảnh"""
        if not os.path.exists(image_path):
            return None, 0.0
        
        try:
            # Đọc ảnh
            image = cv2.imread(image_path)
            if image is None:
                print(f"Could not load image: {image_path}")
                return None, 0.0
            
            rgb_image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
            
            # Tìm khuôn mặt
            face_locations = face_recognition.face_locations(rgb_image, model="hog")
            face_encodings = face_recognition.face_encodings(rgb_image, face_locations)
            
            if not face_encodings:
                return None, 0.0
            
            # So sánh với khuôn mặt đã biết
            for face_encoding in face_encodings:
                distances = face_recognition.face_distance(self.known_face_encodings, face_encoding)
                
                if len(distances) > 0:
                    best_match_index = np.argmin(distances)
                    confidence = 1.0 - distances[best_match_index]
                    
                    if distances[best_match_index] <= self.confidence_threshold:
                        employee_id = self.known_face_names[best_match_index]
                        print(f"Recognized employee {employee_id} with confidence {confidence:.3f}")
                        return employee_id, confidence
            
            return None, 0.0
            
        except Exception as e:
            print(f"Error in face recognition: {e}")
            return None, 0.0
    
    def capture_image(self, output_path, camera_index=0):
        """Chụp ảnh từ camera"""
        cap = cv2.VideoCapture(camera_index)
        if not cap.isOpened():
            print("Error: Could not open camera")
            return False
        
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
        
        # Chờ camera ổn định
        for _ in range(10):
            cap.read()
        
        ret, frame = cap.read()
        if ret:
            frame = cv2.flip(frame, 1)
            # Tạo thư mục nếu chưa có
            os.makedirs(os.path.dirname(output_path), exist_ok=True)
            cv2.imwrite(output_path, frame)
            print(f"Image captured and saved to {output_path}")
            success = True
        else:
            print("Error: Could not capture frame")
            success = False
        
        cap.release()
        return success
    
    def start_camera_stream(self, temp_image_path="temp_capture.jpg", camera_index=0):
        """Bắt đầu camera stream để capture liên tục"""
        self.is_camera_running = True
        
        def camera_loop():
            cap = cv2.VideoCapture(camera_index)
            if not cap.isOpened():
                print("Error: Could not open camera")
                self.is_camera_running = False
                return
            
            cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
            cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
            
            while self.is_camera_running:
                ret, frame = cap.read()
                if not ret:
                    break
                
                frame = cv2.flip(frame, 1)
                cv2.imwrite(temp_image_path, frame)
                time.sleep(0.1)  # 10 FPS
            
            cap.release()
        
        self.capture_thread = threading.Thread(target=camera_loop)
        self.capture_thread.daemon = True
        self.capture_thread.start()
        return True
    
    def stop_camera_stream(self):
        """Dừng camera stream"""
        self.is_camera_running = False
        if self.capture_thread:
            self.capture_thread.join(timeout=2)
    
    def get_statistics(self):
        """Lấy thống kê hệ thống"""
        return {
            'total_registered_faces': len(self.known_face_names),
            'registered_employees': self.known_face_names,
            'encodings_file': self.encodings_file,
            'confidence_threshold': self.confidence_threshold,
            'last_modified': datetime.fromtimestamp(
                os.path.getmtime(self.encodings_file)
            ).isoformat() if os.path.exists(self.encodings_file) else None
        }
    
    def delete_employee_face(self, employee_id):
        """Xóa dữ liệu khuôn mặt của nhân viên"""
        employee_id_str = str(employee_id)
        
        if employee_id_str in self.known_face_names:
            index = self.known_face_names.index(employee_id_str)
            del self.known_face_encodings[index]
            del self.known_face_names[index]
            
            if self.save_known_faces():
                print(f"Deleted face data for employee {employee_id}")
                return True
            else:
                print(f"Failed to save after deleting employee {employee_id}")
                return False
        else:
            print(f"Employee {employee_id} not found in face database")
            return False
    
    def recognize_face_realtime(self, camera_index=0, show_window=True):
        """Nhận diện khuôn mặt thời gian thực"""
        cap = cv2.VideoCapture(camera_index)
        if not cap.isOpened():
            print("Error: Could not open camera")
            return False
        
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
        
        print("Starting real-time face recognition. Press 'q' to quit.")
        
        # Tối ưu hiệu suất
        process_this_frame = True
        recognized_employee = None
        confidence = 0.0
        
        while True:
            ret, frame = cap.read()
            if not ret:
                break
            
            # Lật frame
            frame = cv2.flip(frame, 1)
            
            # Chỉ xử lý mỗi frame thứ 2 để tăng hiệu suất
            if process_this_frame:
                # Giảm kích thước frame để xử lý nhanh hơn
                small_frame = cv2.resize(frame, (0, 0), fx=0.5, fy=0.5)
                rgb_small_frame = cv2.cvtColor(small_frame, cv2.COLOR_BGR2RGB)
                
                # Tìm khuôn mặt
                face_locations = face_recognition.face_locations(rgb_small_frame, model="hog")
                face_encodings = face_recognition.face_encodings(rgb_small_frame, face_locations)
                
                recognized_employee = None
                confidence = 0.0
                
                for face_encoding in face_encodings:
                    distances = face_recognition.face_distance(self.known_face_encodings, face_encoding)
                    
                    if len(distances) > 0:
                        best_match_index = np.argmin(distances)
                        temp_confidence = 1.0 - distances[best_match_index]
                        
                        if distances[best_match_index] <= self.confidence_threshold:
                            recognized_employee = self.known_face_names[best_match_index]
                            confidence = temp_confidence
                            break
            
            process_this_frame = not process_this_frame
            
            if show_window:
                # Hiển thị kết quả
                if recognized_employee:
                    text = f"Employee: {recognized_employee} ({confidence:.2f})"
                    color = (0, 255, 0)
                else:
                    text = "Unknown"
                    color = (0, 0, 255)
                
                cv2.putText(frame, text, (20, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, color, 2)
                cv2.putText(frame, "Press 'q' to quit", (20, frame.shape[0] - 20), 
                           cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 255, 255), 2)
                
                # Vẽ khung khuôn mặt
                if 'face_locations' in locals():
                    for (top, right, bottom, left) in face_locations:
                        # Scale lại tọa độ
                        top *= 2
                        right *= 2
                        bottom *= 2
                        left *= 2
                        
                        cv2.rectangle(frame, (left, top), (right, bottom), color, 2)
                
                cv2.imshow('Face Recognition', frame)
                
                if cv2.waitKey(1) & 0xFF == ord('q'):
                    break
            else:
                # Không hiển thị window, chỉ return kết quả
                if recognized_employee:
                    cap.release()
                    cv2.destroyAllWindows()
                    return recognized_employee, confidence
        
        cap.release()
        cv2.destroyAllWindows()
        return True


def main():
    """Hàm main để test và sử dụng từ command line"""
    parser = argparse.ArgumentParser(description='Face Recognition System for Employee Management')
    parser.add_argument('command', choices=['register', 'recognize', 'realtime', 'capture', 'stats', 'delete', 'stream_start', 'stream_stop'],
                       help='Command to execute')
    parser.add_argument('--employee_id', type=int, help='Employee ID for registration or deletion')
    parser.add_argument('--image_path', help='Path to image for recognition')
    parser.add_argument('--output_path', help='Output path for captured image')
    parser.add_argument('--camera_index', type=int, default=0, help='Camera index (default: 0)')
    parser.add_argument('--confidence', type=float, default=0.6, help='Confidence threshold (default: 0.6)')
    
    if len(sys.argv) < 2:
        print("Usage:")
        print("  python face_recognition_system.py register --employee_id <id>")
        print("  python face_recognition_system.py recognize --image_path <path>")
        print("  python face_recognition_system.py realtime")
        print("  python face_recognition_system.py capture --output_path <path>")
        print("  python face_recognition_system.py stats")
        print("  python face_recognition_system.py delete --employee_id <id>")
        return
    
    args = parser.parse_args()
    
    system = FaceRecognitionSystem(confidence_threshold=args.confidence)
    
    if args.command == "register":
        if not args.employee_id:
            print("Error: Employee ID required for registration")
            sys.exit(1)
        
        success = system.register_face(args.employee_id)
        print("completed successfully" if success else "failed")
        sys.exit(0 if success else 1)
        
    elif args.command == "recognize":
        if not args.image_path:
            print("Error: Image path required for recognition")
            sys.exit(1)
        
        employee_id, confidence = system.recognize_face_from_image(args.image_path)
        
        if employee_id:
            print(f"{employee_id}")  # Output for C# application
        else:
            print("Unknown")
        
    elif args.command == "realtime":
        def recognition_callback(employee_id, confidence):
            print(f"Recognized: {employee_id} (confidence: {confidence:.3f})")
        
        system.recognize_face_realtime(camera_index=args.camera_index)
        
    elif args.command == "capture":
        output_path = args.output_path or "captured_image.jpg"
        success = system.capture_image(output_path, args.camera_index)
        sys.exit(0 if success else 1)
        
    elif args.command == "stats":
        stats = system.get_statistics()
        print(json.dumps(stats, indent=2))
        
    elif args.command == "delete":
        if not args.employee_id:
            print("Error: Employee ID required for deletion")
            sys.exit(1)
        
        success = system.delete_employee_face(args.employee_id)
        sys.exit(0 if success else 1)
        
    elif args.command == "stream_start":
        success = system.start_camera_stream()
        print("Camera stream started" if success else "Failed to start camera stream")
        
    elif args.command == "stream_stop":
        system.stop_camera_stream()
        print("Camera stream stopped")


if __name__ == "__main__":
    main()