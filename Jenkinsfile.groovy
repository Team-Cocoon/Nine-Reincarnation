def PROJECT_NAME = "NineReincarnation"
def UNITY_VERSION = "6000.1.17f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor"

pipeline
{
    options { disableConcurrentBuilds(abortPrevious: true) }

    environment
    {
        KEY_FILE_PATH = 'jenkins-unity-upload-bd27d6a5ec4a.json'

        PROJECT_PATH = "C:\\Git\\Nine-Reincarnation\\${PROJECT_NAME}"

        OUTPUT_WIN = "C:\\Builds\\NineReincarnation\\${STATE}\\WindowBuild\\Window"
        OUTPUT_WEB = "C:\\Builds\\NineReincarnation\\${STATE}\\WebBuild\\Web"

        OUTPUT_WIN_ZIP = "C:\\Builds\\NineReincarnation\\${STATE}\\WindowBuild\\Window.zip"
        OUTPUT_WEB_ZIP = "C:\\Builds\\NineReincarnation\\${STATE}\\WebBuild\\Web.zip"

        UPLOAD_SCRIPT = "upload_to_drive.py"
    }

    agent
    {
        label
        {
            label ""
            customWorkspace "C:\\JenkinsWorkspace\\${STATE}\\NineReincarnation"
        }
    }

    stages
    {
        stage("Build Windows")
        {
            when
            {
                expression {BUILD_WINDOWS == 'true'}
            }
            steps
            {
                script
                {
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"])
                    {
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWindows -buildOutput "%OUTPUT_WIN%" -logfile -
                        '''
                    }
                }
            }
        }

        stage("Deploy Windows")
        {
            when
            {
                expression {DEPLOY_WINDOWS == 'true'}
            }
            steps
            {
                withCredentials([file(credentialsId: 'gdrive-secret-key', variable: 'SECRET_FILE')]) 
                {
                    script {
                        // 키 파일 복사
                        bat "copy /Y \"%SECRET_FILE%\" %KEY_FILE_PATH%"
                        
                        // 파이썬 라이브러리 설치 (필요시)
                        bat "python -m pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib"
                        
                        // 업로드 스크립트 실행 (유니티가 만든 Zip 파일 경로 전달)
                        bat "python ${UPLOAD_SCRIPT} \"%OUTPUT_WIN_ZIP%\""
                        
                        // 보안을 위해 키 파일 삭제
                        bat "del %KEY_FILE_PATH%"
                    }
                }
            }
        }

        stage("Build WebGL")
        {
            when
            {
                expression {BUILD_WEBGL == 'true'}
            }
            steps
            {
                script
                {
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"])
                    {
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWebGL -buildOutput "%OUTPUT_WEB%" -logfile -
                        '''
                    }
                }
            }
        }

        stage("Deploy WebGL")
        {
            when
            {
                expression {DEPLOY_WEBGL == 'true'}
            }
            steps
            {
                withCredentials([file(credentialsId: 'gdrive-secret-key', variable: 'SECRET_FILE')]) 
                {
                    script {
                        bat "copy /Y \"%SECRET_FILE%\" %KEY_FILE_PATH%"

                        bat "python -m pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib"
                        
                        // WebGL Zip 파일 업로드
                        bat "python ${UPLOAD_SCRIPT} \"%OUTPUT_WEB_ZIP%\""
                        
                        bat "del %KEY_FILE_PATH%"
                    }
                }
            }
        }
    }

    post 
    {
        success 
        {
            script 
            {
                // 1. Git 정보 가져오기 (아래 정의한 @NonCPS 함수 호출)
                def gitData = getGitChanges() 
                
                // 2. 현재 시간 구하기 (이게 없으면 에러 납니다)
                def buildTime = new Date().format("yyyy-MM-dd HH:mm:ss", TimeZone.getTimeZone("Asia/Seoul"))
                
                // 3. 디스코드 전송
                withCredentials([string(credentialsId: 'Discord-Webhook', variable: 'DISCORD')]) 
                {
                    discordSend description: """
                    **[빌드 성공]**
                    **결과** : ${currentBuild.result}
                    📝 **커밋**: ${gitData.msg}
                    👤 **작성자**: ${gitData.author}
                    📅 **일시**: ${buildTime}
                    ⏱ **실행 시간** : ${currentBuild.durationString.replace(' and counting', '')}
                    """,
                    link: env.BUILD_URL, 
                    result: currentBuild.currentResult, 
                    title: "✅ ${env.JOB_NAME} 빌드 완료", 
                    webhookURL: "$DISCORD"
                }
            }
        }

        failure {
            script {

                def buildTime = new Date().format("yyyy-MM-dd HH:mm:ss", TimeZone.getTimeZone("Asia/Seoul"))
                
                withCredentials([string(credentialsId: 'Discord-Webhook', variable: 'DISCORD')]) {
                    discordSend description: """
                    **결과** : ${currentBuild.result}
                    📅 **일시**: ${buildTime}
                    ⏱ **실행 시간** : ${currentBuild.durationString}
                    """,
                    link: env.BUILD_URL, result: currentBuild.currentResult, 
                    title: "${env.JOB_NAME} : ${currentBuild.displayName} 실패", 
                    webhookURL: "$DISCORD"
                }
            }
        }
    }
}

@NonCPS
def getGitChanges() {
    def commitMsg = "변경 사항 없음 (수동 빌드 또는 재시도)"
    def commitAuthor = "Unknown"
    
    try {
        def changeLogSets = currentBuild.changeSets
        if (changeLogSets.size() > 0) {
            def entries = changeLogSets[0].items
            if (entries.length > 0) {
                def lastEntry = entries[entries.length - 1]
                commitMsg = lastEntry.msg
                commitAuthor = lastEntry.author.fullName
            }
        }
    } catch (Exception e) {
        commitMsg = "커밋 정보 불러오기 실패: ${e.message}"
    }
    
    return [msg: commitMsg, author: commitAuthor]
}
