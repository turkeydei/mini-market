#!/bin/bash

# MiniShop AWS Deployment Script
# Usage: ./deploy.sh [environment] [action]
# Example: ./deploy.sh dev deploy

set -e

ENVIRONMENT=${1:-dev}
ACTION=${2:-deploy}
STACK_NAME="minishop-${ENVIRONMENT}"

echo "🚀 Starting MiniShop deployment..."
echo "Environment: $ENVIRONMENT"
echo "Action: $ACTION"
echo "Stack Name: $STACK_NAME"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if AWS CLI is installed
if ! command -v aws &> /dev/null; then
    print_error "AWS CLI is not installed. Please install it first."
    exit 1
fi

# Check if AWS credentials are configured
if ! aws sts get-caller-identity &> /dev/null; then
    print_error "AWS credentials not configured. Run 'aws configure' first."
    exit 1
fi

# Function to deploy infrastructure
deploy_infrastructure() {
    print_status "Deploying CloudFormation stack..."
    
    aws cloudformation deploy \
        --template-file ../AWS-Infrastructure/cloudformation-template.yaml \
        --stack-name $STACK_NAME \
        --parameter-overrides Environment=$ENVIRONMENT \
        --capabilities CAPABILITY_IAM \
        --region ap-southeast-1
    
    print_status "Infrastructure deployed successfully!"
}

# Function to deploy Lambda functions
deploy_lambdas() {
    print_status "Deploying Lambda functions..."
    
    # Get API Gateway URL from CloudFormation
    API_URL=$(aws cloudformation describe-stacks \
        --stack-name $STACK_NAME \
        --query 'Stacks[0].Outputs[?OutputKey==`APIGatewayURL`].OutputValue' \
        --output text)
    
    print_status "API Gateway URL: $API_URL"
    
    # Deploy each Lambda function
    for function in products checkout orders; do
        print_status "Deploying $function Lambda..."
        
        cd ../AWS-Lambda/functions/$function
        
        # Install dependencies
        npm install
        
        # Create deployment package
        zip -r function.zip .
        
        # Update Lambda function code
        aws lambda update-function-code \
            --function-name "minishop-${ENVIRONMENT}-${function}" \
            --zip-file fileb://function.zip \
            --region ap-southeast-1
        
        # Clean up
        rm function.zip
        cd ../../../AWS-Deploy
    done
    
    print_status "Lambda functions deployed successfully!"
}

# Function to deploy frontend
deploy_frontend() {
    print_status "Deploying frontend to S3..."
    
    # Get S3 bucket name from CloudFormation
    BUCKET_NAME=$(aws cloudformation describe-stacks \
        --stack-name $STACK_NAME \
        --query 'Stacks[0].Outputs[?OutputKey==`FrontendBucket`].OutputValue' \
        --output text)
    
    print_status "Frontend bucket: $BUCKET_NAME"
    
    # Build React app
    cd ../AWS-Frontend
    npm install
    npm run build
    
    # Upload to S3
    aws s3 sync build/ s3://$BUCKET_NAME --delete
    
    # Set bucket policy for public read
    aws s3api put-bucket-policy --bucket $BUCKET_NAME --policy '{
        "Version": "2012-10-17",
        "Statement": [
            {
                "Sid": "PublicReadGetObject",
                "Effect": "Allow",
                "Principal": "*",
                "Action": "s3:GetObject",
                "Resource": "arn:aws:s3:::'$BUCKET_NAME'/*"
            }
        ]
    }'
    
    cd ../AWS-Deploy
    print_status "Frontend deployed successfully!"
}

# Function to setup database
setup_database() {
    print_status "Setting up MySQL database..."
    
    # Get RDS endpoint
    RDS_ENDPOINT=$(aws cloudformation describe-stacks \
        --stack-name $STACK_NAME \
        --query 'Stacks[0].Outputs[?OutputKey==`RDSConnectionString`].OutputValue' \
        --output text | cut -d'@' -f2 | cut -d':' -f1)
    
    print_status "RDS Endpoint: $RDS_ENDPOINT"
    
    # Wait for RDS to be available
    print_status "Waiting for RDS to be available..."
    aws rds wait db-instance-available --db-instance-identifier "minishop-${ENVIRONMENT}-db"
    
    # Run database schema
    print_status "Creating database schema..."
    mysql -h $RDS_ENDPOINT -u admin -p'YourPassword123!' < ../AWS-Database/mysql-schema.sql
    
    print_status "Database setup completed!"
}

# Function to cleanup
cleanup() {
    print_warning "Cleaning up resources..."
    
    # Delete CloudFormation stack
    aws cloudformation delete-stack --stack-name $STACK_NAME
    
    # Wait for stack deletion
    aws cloudformation wait stack-delete-complete --stack-name $STACK_NAME
    
    print_status "Cleanup completed!"
}

# Main execution
case $ACTION in
    "deploy")
        deploy_infrastructure
        setup_database
        deploy_lambdas
        deploy_frontend
        print_status "🎉 Deployment completed successfully!"
        ;;
    "update")
        deploy_infrastructure
        deploy_lambdas
        deploy_frontend
        print_status "🔄 Update completed successfully!"
        ;;
    "cleanup")
        cleanup
        ;;
    *)
        print_error "Invalid action. Use: deploy, update, or cleanup"
        exit 1
        ;;
esac

print_status "Deployment script finished!"
