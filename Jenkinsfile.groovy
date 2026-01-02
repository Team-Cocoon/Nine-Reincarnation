def PROJECT_NAME = "NineReincarnation"
def CUSTOM_WORKSPACE = "C:\\Git\\${PROJECT_NAME}"
def UNITY_VERSION = "6000.1.17f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor"

pipeline
{
    environment
    {
        PROJECT_PATH = "${CUSTOM_WORKSPACE}\\${PROJECT_NAME}"
    }

    agent
    {
        label
        {
            label ""
            customWorkspace "${CUSTOM_WORKSPACE}"
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

    }
}
