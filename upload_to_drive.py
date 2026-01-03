import os
import sys
from google.oauth2 import service_account
from googleapiclient.discovery import build
from googleapiclient.http import MediaFileUpload

# --- 설정 부분 ---
KEY_FILE_PATH = 'jenkins-unity-upload-bd27d6a5ec4a.json'
FOLDER_ID = '1Jhi7R7-99fjV7AAYoUnbEVDb7zIg6nZT' # 본인의 폴더 ID
SCOPES = ['https://www.googleapis.com/auth/drive']

def get_drive_service():
    creds = service_account.Credentials.from_service_account_file(
        KEY_FILE_PATH, scopes=SCOPES)
    return build('drive', 'v3', credentials=creds)

def delete_same_name_files(service, target_name):
    """
    구글 드라이브 폴더 내에서 'target_name'과 이름이 같은 파일을 모두 찾아 삭제
    """
    print(f"중복 파일 검색 중: {target_name}...")
    
    try:
        # 쿼리: 이름이 같고 + 해당 폴더에 있고 + 휴지통에 없는 파일
        query = f"name = '{target_name}' and '{FOLDER_ID}' in parents and trashed=false"
        
        results = service.files().list(q=query, fields="files(id, name)").execute()
        files = results.get('files', [])

        if not files:
            print("- 중복된 파일이 없습니다. 바로 업로드합니다.")
            return

        for file in files:
            print(f"- 기존 파일 삭제 중... (ID: {file['id']})")
            service.files().delete(fileId=file['id']).execute()
            
        print("- 기존 중복 파일 정리 완료.")

    except Exception as e:
        print(f"파일 검색/삭제 중 오류 발생: {e}")
        # 오류가 나도 업로드는 시도하고 싶다면 아래 줄을 삭제
        sys.exit(1)

def upload_file(file_path):
    if not os.path.exists(file_path):
        print(f"Error: 업로드할 파일이 없습니다 -> {file_path}")
        sys.exit(1)

    try:
        service = get_drive_service()
        file_name = os.path.basename(file_path)

        # 이름이 같은 기존 파일만 삭제 (덮어쓰기 효과)
        delete_same_name_files(service, file_name)

        # 새 파일 업로드
        file_metadata = {
            'name': file_name,
            'parents': [FOLDER_ID]
        }
        
        media = MediaFileUpload(file_path, resumable=True)

        print(f"새 파일 업로드 시작: {file_name}...")
        
        file = service.files().create(body=file_metadata,
                                      media_body=media,
                                      fields='id').execute()
                                      
        print(f"성공! 업로드 완료. File ID: {file.get('id')}")

    except Exception as e:
        print(f"업로드 실패: {e}")
        sys.exit(1)

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("사용법: python upload_to_drive.py [업로드할_파일경로]")
    else:
        upload_file(sys.argv[1])