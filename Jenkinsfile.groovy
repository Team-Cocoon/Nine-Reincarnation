def PROJECT_NAME = "NineReincarnation"
def UNITY_VERSION = "6000.1.17f1"
def UNITY_INSTALLATION = "C:/Program Files/Unity/Hub/Editor/${UNITY_VERSION}/Editor"

pipeline
{
    options { disableConcurrentBuilds(abortPrevious: true) }

    environment
    {
        // 여기서는 ${params.STATE}를 쓰지 않습니다. (나중에 script 안에서 정의)
        KEY_FILE_PATH = 'jenkins-unity-upload-bd27d6a5ec4a.json'
        PROJECT_PATH_SUFFIX = "NineReincarnation/${PROJECT_NAME}" // 경로 조합용 접미사
        UPLOAD_SCRIPT = "upload_to_drive.py"
    }

    agent
    {
        label
        {
            // [중요] 여기서 customWorkspace를 쓰지 마세요. 파라미터가 안 먹힙니다.
            label "" 
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
                    // 1. 여기서 동적 경로를 정의합니다. (이 시점에는 params.STATE가 100% 존재함)
                    def WORKSPACE_PATH = "C:/JenkinsWorkspace/${STATE}/NineReincarnation"
                    def OUTPUT_WIN = "C:/Builds/NineReincarnation/${STATE}/WindowBuild/Window"
                    
                    echo "Current Workspace: ${WORKSPACE_PATH}"

                    // 2. ws() 블록으로 작업 공간을 강제 이동합니다.
                    ws(WORKSPACE_PATH)
                    {
                        // 3. [매우 중요] 작업 공간을 옮겼으니 소스코드를 가져옵니다.
                        checkout scm
                        
                        // 4. 프로젝트 경로 재설정 (현재 ws 기준)
                        def CURRENT_PROJECT_PATH = "${WORKSPACE_PATH}/${PROJECT_NAME}"

                        withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"])
                        {
                            bat "taskkill /F /IM Unity.exe || exit 0"
                            
                            bat """
                            "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath "${CURRENT_PROJECT_PATH}" -executeMethod BuildScript.BuildWindows -buildOutput "${OUTPUT_WIN}" -logfile -
                            """
                        }
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
                script 
                {
                    // 배포 단계도 같은 워크스페이스를 써야 파일(업로드 스크립트 등)을 찾을 수 있습니다.
                    def WORKSPACE_PATH = "C:/JenkinsWorkspace/${STATE}/NineReincarnation"
                    def OUTPUT_WIN_ZIP = "C:/Builds/NineReincarnation/${STATE}/WindowBuild/Window.zip"

                    ws(WORKSPACE_PATH)
                    {
                        withCredentials([file(credentialsId: 'gdrive-secret-key', variable: 'SECRET_FILE')]) 
                        {
                            bat "copy /Y \"%SECRET_FILE%\" %KEY_FILE_PATH%"
                            bat "python -m pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib"
                            
                            bat "python ${UPLOAD_SCRIPT} \"${OUTPUT_WIN_ZIP}\""
                            
                            bat "del %KEY_FILE_PATH%"
                        }
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
                    def WORKSPACE_PATH = "C:/JenkinsWorkspace/${STATE}/NineReincarnation"
                    def OUTPUT_WEB = "C:/Builds/NineReincarnation/${STATE}/WebBuild/Web"

                    ws(WORKSPACE_PATH)
                    {
                        // WebGL만 단독 빌드할 수도 있으므로 여기서도 체크아웃
                        checkout scm 
                        
                        def CURRENT_PROJECT_PATH = "${WORKSPACE_PATH}/${PROJECT_NAME}"

                        withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"])
                        {
                            bat "taskkill /F /IM Unity.exe || exit 0"
                            
                            bat """
                            "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath "${CURRENT_PROJECT_PATH}" -executeMethod BuildScript.BuildWebGL -buildOutput "${OUTPUT_WEB}" -logfile -
                            """
                        }
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
                script 
                {
                    def WORKSPACE_PATH = "C:/JenkinsWorkspace/${STATE}/NineReincarnation"
                    def OUTPUT_WEB_ZIP = "C:/Builds/NineReincarnation/${STATE}/WebBuild/Web.zip"
                    
                    ws(WORKSPACE_PATH)
                    {
                        withCredentials([file(credentialsId: 'gdrive-secret-key', variable: 'SECRET_FILE')]) 
                        {
                            bat "copy /Y \"%SECRET_FILE%\" %KEY_FILE_PATH%"
                            bat "python -m pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib"
                            bat "python ${UPLOAD_SCRIPT} \"${OUTPUT_WEB_ZIP}\""
                            bat "del %KEY_FILE_PATH%"
                        }
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
                def gitData = getGitChanges() 
                def buildTime = new Date().format("yyyy-MM-dd HH:mm:ss", TimeZone.getTimeZone("Asia/Seoul"))
                
                withCredentials([string(credentialsId: 'Discord-Webhook', variable: 'DISCORD')]) 
                {
                    discordSend description: """
                    **[빌드 성공]** (${params.STATE})
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