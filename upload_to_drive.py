import os
import sys
from google.oauth2 import service_account
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload

# --- 설정 확인 ---
# 젠킨스에서 복사되는 파일명과 정확히 일치해야 합니다.
KEY_FILE_PATH = 'jenkins-unity-upload-bd27d6a5ec4a.json' 
# 본인의 폴더 ID (공유 설정한 그 폴더)
FOLDER_ID = '1Jhi7R7-99fjV7AAYoUnbEVDb7zIg6nZT' 
SCOPES = ['https://www.googleapis.com/auth/drive']

def debug_and_upload(file_path):
    if not os.path.exists(KEY_FILE_PATH):
        print(f"[ERROR] Key file not found! ({KEY_FILE_PATH})")
        print(f"Current Dir: {os.getcwd()}")
        sys.exit(1)

    try:
        # 1. 인증 시도
        creds = service_account.Credentials.from_service_account_file(
            KEY_FILE_PATH, scopes=SCOPES)
        service = build('drive', 'v3', credentials=creds)

        # 2. [범인 찾기] 현재 이 키 파일의 주인은 누구인가?
        about = service.about().get(fields="user").execute()
        current_email = about['user']['emailAddress']
        print("=========================================")
        print(f"[INFO] Current Robot Email: {current_email}")
        print("=========================================")

        # 3. [권한 확인] 공유된 폴더가 보이는가?
        try:
            folder = service.files().get(fileId=FOLDER_ID, fields="name").execute()
            print(f"[SUCCESS] Folder Access OK! Name: {folder.get('name')}")
        except Exception as e:
            print(f"[FAIL] Folder Access Failed!")
            print(f"   -> Folder ID: {FOLDER_ID}")
            print(f"   -> Check if '{current_email}' is invited as 'Editor'.")
            # 여기서 진짜 에러 내용을 봅니다 (이모지 없음)
            print(f"   -> Real Google Error: {e}")
            sys.exit(1)

        # 4. 파일 업로드 시작
        file_name = os.path.basename(file_path)
        
        # 중복 파일 삭제 로직
        query = f"name = '{file_name}' and '{FOLDER_ID}' in parents and trashed=false"
        results = service.files().list(q=query, fields="files(id)").execute()
        for f in results.get('files', []):
            print(f"[INFO] Deleting old file: {f['id']}")
            service.files().delete(fileId=f['id']).execute()

        # 업로드
        file_metadata = {'name': file_name, 'parents': [FOLDER_ID]}
        media = MediaFileUpload(file_path, resumable=True)

        print(f"[INFO] Uploading: {file_name}")
        file = service.files().create(body=file_metadata, media_body=media, fields='id').execute()
        print(f"[SUCCESS] Upload Complete! File ID: {file.get('id')}")

    except Exception as e:
        # 이모지 제거됨
        print(f"[CRITICAL ERROR] : {e}")
        sys.exit(1)

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("Usage: python upload_to_drive.py [FilePath]")
    else:
        debug_and_upload(sys.argv[1])