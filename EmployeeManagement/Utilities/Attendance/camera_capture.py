"""
Script chụp ảnh từ camera cho hệ thống chấm công
"""

import cv2
import sys
import os
import time

def capture_frame(output_path="temp_capture.jpg", camera_index=0):
    """Chụp một frame từ camera"""
    cap = cv2.VideoCapture(camera_index)
    
    if not cap.isOpened():
        print('Error: Could not open camera')
        return False
    
    # Cài đặt camera
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
    
    # Đợi camera ổn định
    for _ in range(5):
        cap.read()
    
    ret, frame = cap.read()
    if ret:
        # Lật frame theo chiều ngang
        frame = cv2.flip(frame, 1)
        
        # Tạo thư mục nếu cần
        os.makedirs(os.path.dirname(output_path) if os.path.dirname(output_path) else '.', exist_ok=True)
        
        cv2.imwrite(output_path, frame)
        print(f'Frame captured successfully: {output_path}')
        success = True
    else:
        print('Error: Could not capture frame')
        success = False
    
    cap.release()
    return success

def start_camera_preview(camera_index=0):
    """Hiển thị preview từ camera"""
    cap = cv2.VideoCapture(camera_index)
    
    if not cap.isOpened():
        print('Error: Could not open camera')
        return False
    
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
    
    print("Camera preview started. Press 'c' to capture, 'q' to quit")
    
    capture_count = 0
    
    while True:
        ret, frame = cap.read()
        if not ret:
            print('Error: Could not read frame')
            break
        
        # Lật frame
        frame = cv2.flip(frame, 1)
        
        # Hiển thị hướng dẫn
        cv2.putText(frame, "Press 'c' to capture, 'q' to quit", 
                   (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (255, 255, 255), 2)
        cv2.putText(frame, f"Captures: {capture_count}", 
                   (10, 60), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 255, 0), 2)
        
        cv2.imshow('Camera Preview', frame)
        
        key = cv2.waitKey(1) & 0xFF
        if key == ord('q'):
            break
        elif key == ord('c'):
            # Chụp ảnh
            timestamp = time.strftime("%Y%m%d_%H%M%S")
            filename = f"capture_{timestamp}.jpg"
            cv2.imwrite(filename, frame)
            capture_count += 1
            print(f"Captured: {filename}")
    
    cap.release()
    cv2.destroyAllWindows()
    print("Camera preview ended")
    return True

def continuous_capture(interval=1.0, duration=10.0, camera_index=0):
    """Chụp ảnh liên tục với khoảng thời gian nhất định"""
    cap = cv2.VideoCapture(camera_index)
    
    if not cap.isOpened():
        print('Error: Could not open camera')
        return False
    
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)
    
    start_time = time.time()
    last_capture = 0
    capture_count = 0
    
    print(f"Starting continuous capture for {duration}s with {interval}s interval")
    
    # Tạo thư mục cho ảnh
    capture_dir = f"continuous_capture_{int(start_time)}"
    os.makedirs(capture_dir, exist_ok=True)
    
    while time.time() - start_time < duration:
        ret, frame = cap.read()
        if not ret:
            continue
        
        current_time = time.time()
        
        # Chụp ảnh theo interval
        if current_time - last_capture >= interval:
            frame = cv2.flip(frame, 1)
            filename = os.path.join(capture_dir, f"frame_{capture_count:04d}.jpg")
            cv2.imwrite(filename, frame)
            capture_count += 1
            last_capture = current_time
            print(f"Captured frame {capture_count}: {filename}")
        
        # Hiển thị preview (optional)
        frame_display = cv2.flip(frame, 1) if ret else None
        if frame_display is not None:
            cv2.putText(frame_display, f"Captures: {capture_count}", 
                       (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
            cv2.putText(frame_display, f"Time: {current_time - start_time:.1f}s", 
                       (10, 70), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 255, 0), 2)
            cv2.imshow('Continuous Capture', frame_display)
        
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    
    cap.release()
    cv2.destroyAllWindows()
    
    print(f"Continuous capture completed. {capture_count} frames saved to {capture_dir}")
    return True

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage:")
        print("  python camera_capture.py capture [output_path]")
        print("  python camera_capture.py preview")
        print("  python camera_capture.py continuous [interval] [duration]")
        sys.exit(1)
    
    command = sys.argv[1]
    
    if command == "capture":
        output_path = sys.argv[2] if len(sys.argv) > 2 else "temp_capture.jpg"
        success = capture_frame(output_path)
        sys.exit(0 if success else 1)
        
    elif command == "preview":
        camera_index = int(sys.argv[2]) if len(sys.argv) > 2 else 0
        success = start_camera_preview(camera_index)
        sys.exit(0 if success else 1)
        
    elif command == "continuous":
        interval = float(sys.argv[2]) if len(sys.argv) > 2 else 1.0
        duration = float(sys.argv[3]) if len(sys.argv) > 3 else 10.0
        camera_index = int(sys.argv[4]) if len(sys.argv) > 4 else 0
        success = continuous_capture(interval, duration, camera_index)
        sys.exit(0 if success else 1)
        
    else:
        print(f"Unknown command: {command}")
        sys.exit(1)