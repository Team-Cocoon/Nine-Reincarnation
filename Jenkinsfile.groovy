def PROJECT_NAME = "Nine-Reincarnation"
def CUSTOM_WORKSPCE = "C:\\Git\\${PROJECT_NAME}"
def UNITY_VERSION = "6000.1.17f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor\\Unity.exe"

pipline
{
    environmant
    {
        PROJECT_PATH = "${CUSTOM_WORKSPACE}\\${PROJECT_NAME}"
    }

    agent
    {
        label
        {
            label ""
            customWorkspace "${CUSTOM_WORKSPCE}"
        }
    }

    stages
    {
        stages("Build Windows")
        {
            when(expression {BUILD_WINDOWS = 'true'})
            stages
            {
                script
                {
                    withEnv(["UNITY_PATH${UNITY_INSTALLATION}"])
                    {
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -excuteMethod BuildScript.BuildWindows -logfile -
                        '''
                    }
                }
            }
        }

        stages("Deploy Windows")
        {
            whie(expression {DEPLOY_WINDOWS == 'true'})
            stages
            {
                echo 'Deploy Windows'
            }
        }

    }
}
