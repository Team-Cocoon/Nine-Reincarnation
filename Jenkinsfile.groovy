def PROJECT_NAME = "NineReincarnation"
def UNITY_VERSION = "6000.1.17f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor"

pipeline
{
    environment
    {
        PROJECT_PATH = "${CUSTOM_WORKSPACE}\\${PROJECT_NAME}"

        OUTPUT_WIN = "C:\\Builds\\NineReincarnation\\Window\\NineReincarnation.exe"
        OUTPUT_WEB = "C:\\Builds\\NineReincarnation\\Web"
    }

    parameters {
        booleanParam(name: 'BUILD_WINDOWS', defaultValue: true, description: '윈도우 빌드 수행')
        booleanParam(name: 'DEPLOY_WINDOWS', defaultValue: false, description: '윈도우 배포 수행')
        booleanParam(name: 'BUILD_WEBGL', defaultValue: true, description: 'WebGL 빌드 수행')
        booleanParam(name: 'DEPLOY_WEBGL', defaultValue: false, description: 'WebGL 배포 수행')
    }

    agent
    {
        label
        {
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
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"])
                    {
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWindows -logfile -
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
                echo 'Deploy Windows'
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
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWindows -logfile -
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
                echo 'Deploy Windows'
            }
        }
    }

    post 
    {
        success 
        {
            script 
            {
                // --- 1. 변수 계산 (여기서 먼저 계산합니다) ---
                def commitMsg = "변경 사항 없음 (수동 빌드 또는 재시도)"
                def commitAuthor = "Unknown"
                
                // changeSets가 리스트라 복잡해서 안전하게 가져오는 로직
                def changeLogSets = currentBuild.changeSets
                if (changeLogSets.size() > 0) {
                    def entries = changeLogSets[0].items
                    if (entries.length > 0) {
                        def lastEntry = entries[entries.length - 1]
                        commitMsg = lastEntry.msg
                        commitAuthor = lastEntry.author.fullName
                    }
                }

                // --- 2. 디스코드 전송 (같은 블록 안이라 변수 사용 가능) ---
                withCredentials([string(credentialsId: 'Discord-Webhook', variable: 'DISCORD')]) 
                {
                    discordSend description: """
                    **제목** : ${currentBuild.displayName}
                    **결과** : ${currentBuild.result}
                    📝 **커밋**: ${commitMsg}
                    👤 **작성자**: ${commitAuthor}
                    ⏱ **시간**: ${currentBuild.durationString.replace(' and counting', '')}
                    ⏱ **실행 시간** : ${currentBuild.duration / 1000}s
                    """,
                    link: env.BUILD_URL, 
                    result: currentBuild.currentResult, 
                    title: "✅ ${env.JOB_NAME} 빌드 완료", 
                    webhookURL: "$DISCORD"
                }
            }
        }

        failure {
            withCredentials([string(credentialsId: 'Discord-Webhook', variable: 'DISCORD')]) {
                        discordSend description: """
                        **제목** : ${currentBuild.displayName}
                        **결과** : ${currentBuild.result}
                        ⏱ **시간**: ${currentBuild.durationString.replace(' and counting', '')}
                        ⏱ **실행 시간** : ${currentBuild.duration / 1000}s
                        """,
                        link: env.BUILD_URL, result: currentBuild.currentResult, 
                        title: "${env.JOB_NAME} : ${currentBuild.displayName} 실패", 
                        webhookURL: "$DISCORD"
            }
        }
    }
}
