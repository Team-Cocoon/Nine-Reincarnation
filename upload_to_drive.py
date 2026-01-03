import os
import sys
from google.oauth2 import service_account
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload

# --- 설정 정보 ---
# 젠킨스 환경 변수에서 파일명을 가져오거나, 없으면 기본값 사용
KEY_FILE_PATH = 'jenkins-unity-upload-bd27d6a5ec4a.json'
FOLDER_ID = '1Jhi7R7-99fjV7AAYoUnbEVDb7zIg6nZT' 
SCOPES = ['https://www.googleapis.com/auth/drive']

def upload_by_update(file_path):
    if not os.path.exists(KEY_FILE_PATH):
        print(f"[ERROR] Key file not found: {KEY_FILE_PATH}")
        sys.exit(1)

    try:
        # 1. 인증
        creds = service_account.Credentials.from_service_account_file(
            KEY_FILE_PATH, scopes=SCOPES)
        service = build('drive', 'v3', credentials=creds)

        file_name = os.path.basename(file_path)
        print(f"--------------------------------------------------")
        print(f"[INFO] Processing File: {file_name}")

        # 2. 구글 드라이브에서 '같은 이름의 파일' 찾기
        # (주의: 로봇이 '새로 만들기'를 하면 용량 부족 에러가 뜨므로, 있는 파일을 '수정'해야 함)
        query = f"name = '{file_name}' and '{FOLDER_ID}' in parents and trashed=false"
        results = service.files().list(q=query, fields="files(id, name)").execute()
        files = results.get('files', [])

        if not files:
            print(f"[ERROR] File '{file_name}' not found in the Google Drive folder!")
            print(f"[ACTION REQUIRED] Please upload a dummy file named '{file_name}' to the folder manually first.")
            print("The robot (Service Account) has 0GB quota, so it can only UPDATE your existing files.")
            sys.exit(1)

        # 3. 찾은 파일 덮어쓰기 (Update)
        target_file_id = files[0]['id']
        print(f"[INFO] Found existing file (ID: {target_file_id}). Overwriting content...")

        media = MediaFileUpload(file_path, resumable=True)

        updated_file = service.files().update(
            fileId=target_file_id,
            media_body=media,
            fields='id'
        ).execute()

        print(f"[SUCCESS] Upload (Update) Complete! File ID: {updated_file.get('id')}")
        print(f"--------------------------------------------------")

    except Exception as e:
        print(f"[CRITICAL ERROR] Python Script Failed: {e}")
        sys.exit(1)

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("Usage: python upload_to_drive.py [FilePath]")
    else:
        upload_by_update(sys.argv[1])