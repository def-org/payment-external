pipeline {
    
    agent none

    environment {
        COMPOSE_PROJECT_NAME = "${env.JOB_NAME}-${env.BUILD_ID}"
    }
    stages {
      
        stage('Build') {

            agent any

            steps {
                
                echo sh(script: 'env|sort', returnStdout: true)

                sh  '''
                    docker-compose -f ./docker-compose.yml -f ./docker-compose.prd.yml build
               
                '''

            }

        }

        stage('Publish') {

            agent any

            when { buildingTag() }


            steps {
                
                sh  '''

                     cp ./docker-compose.yml   /docker/payment-external
                     cp ./docker-compose.prd.yml /docker/payment-external

                    docker service update --image externalpaymentgateway:${BRANCH_NAME} external.paymentgateway
               
                '''



                sh  '''

                     /docker/payment-external #


                    

                    docker-compose -f ./docker-compose.yml -f ./docker-compose.prd.yml build
               
                '''

            }

        }


 
    }
    post {

        always {
            node('master'){
                
                sh  '''
               
                '''
            }
        }
    }
}