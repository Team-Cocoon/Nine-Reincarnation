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
        print(f"Error: 키 파일이 없습니다! ({KEY_FILE_PATH})")
        print(f"현재 폴더 위치: {os.getcwd()}")
        print(f"현재 폴더 내용물: {os.listdir('.')}")
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
        print(f"🤖 현재 접속한 로봇(이메일): {current_email}")
        print("=========================================")

        # 3. [권한 확인] 공유된 폴더가 보이는가?
        try:
            folder = service.files().get(fileId=FOLDER_ID, fields="name").execute()
            print(f"✅ 폴더 접속 성공! 폴더명: {folder.get('name')}")
        except Exception as e:
            print(f"❌ 폴더 접속 실패! 로봇이 이 폴더를 볼 수 없습니다.")
            print(f"   -> 공유된 폴더 ID: {FOLDER_ID}")
            print(f"   -> 로봇 이메일({current_email})이 '편집자'로 초대되었는지 확인하세요.")
            sys.exit(1)

        # 4. 파일 업로드 시작
        file_name = os.path.basename(file_path)
        
        # 중복 파일 삭제 로직
        query = f"name = '{file_name}' and '{FOLDER_ID}' in parents and trashed=false"
        results = service.files().list(q=query, fields="files(id)").execute()
        for f in results.get('files', []):
            print(f"- 기존 파일 삭제: {f['id']}")
            service.files().delete(fileId=f['id']).execute()

        # 업로드
        file_metadata = {'name': file_name, 'parents': [FOLDER_ID]}
        media = MediaFileUpload(file_path, resumable=True)

        print(f"📤 업로드 시작: {file_name}")
        file = service.files().create(body=file_metadata, media_body=media, fields='id').execute()
        print(f"🎉 성공! File ID: {file.get('id')}")

    except Exception as e:
        print(f"❌ 에러 발생: {e}")
        sys.exit(1)

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("사용법: python upload_to_drive.py [파일경로]")
    else:
        debug_and_upload(sys.argv[1])