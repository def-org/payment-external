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

                    docker push proget.luizcarlosfaria.com/docker-private-registry/externalpaymentgateway:${BRANCH_NAME}

                    docker service update --image proget.luizcarlosfaria.com/docker-private-registry/externalpaymentgateway:${BRANCH_NAME} external_payment_external_paymentgateway
               
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