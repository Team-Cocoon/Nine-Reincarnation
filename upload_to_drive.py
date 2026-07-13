import os
import sys
from google.oauth2 import service_account
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload

# --- 설정 정보 ---
KEY_FILE_PATH = 'jenkins-unity-upload-bd27d6a5ec4a.json'
SCOPES = ['https://www.googleapis.com/auth/drive']

# STATE 값에 따른 구글 드라이브 폴더 ID 매핑
FOLDER_MAP = {
    'Deploy': os.getenv('ENV_FOLDER_ID_DEPLOY'),     
    'Test': os.getenv('ENV_FOLDER_ID_TEST')
}

def get_folder_id_by_state():
    """젠킨스 환경변수 STATE를 읽어 해당되는 폴더 ID를 반환"""
    # 1. 젠킨스에서 전달된 STATE 환경변수 읽기 (기본값: Dev)
    state = os.environ.get('STATE', 'Deploy')
    
    # 2. 매핑된 ID 찾기
    folder_id = FOLDER_MAP.get(state)
    
    if not folder_id:
        print(f"[ERROR] No Folder ID mapped for STATE: '{state}'")
        print(f"[CHECK] Please update FOLDER_MAP in the python script.")
        sys.exit(1)
        
    print(f"[INFO] Current STATE: {state} -> Target Folder ID: {folder_id}")
    return folder_id

def upload_by_update(file_path):
    if not os.path.exists(KEY_FILE_PATH):
        print(f"[ERROR] Key file not found: {KEY_FILE_PATH}")
        sys.exit(1)

    #동적으로 폴더 ID 가져오기
    target_folder_id = get_folder_id_by_state()

    try:
        # 1. 인증
        creds = service_account.Credentials.from_service_account_file(
            KEY_FILE_PATH, scopes=SCOPES)
        service = build('drive', 'v3', credentials=creds)

        file_name = os.path.basename(file_path)
        print(f"--------------------------------------------------")
        print(f"[INFO] Processing File: {file_name}")

        # 2. 구글 드라이브에서 '해당 폴더(target_folder_id) 안의' 파일 찾기
        # query 문자열 안에 target_folder_id를 넣습니다.
        query = f"name = '{file_name}' and '{target_folder_id}' in parents and trashed=false"
        
        results = service.files().list(q=query, fields="files(id, name)").execute()
        files = results.get('files', [])

        if not files:
            print(f"[ERROR] File '{file_name}' not found in the Google Drive folder ({target_folder_id})!")
            print(f"[ACTION REQUIRED] Please upload a dummy file named '{file_name}' to the '{os.environ.get('STATE', 'Dev')}' folder manually first.")
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