# Hướng Dẫn Deploy Mini Market lên AWS

## 📋 Tổng Quan Kiến Trúc

Kiến trúc AWS của bạn bao gồm:
- **Route 53**: DNS service
- **CloudFront**: CDN và content delivery
- **Amazon S3**: Lưu trữ static assets
- **Amazon WAF**: Bảo mật và bảo vệ ứng dụng
- **VPC**: Virtual Private Cloud với public/private subnets
- **Application Load Balancer**: Phân tải traffic
- **Elastic Beanstalk**: Host ứng dụng .NET
- **Amazon RDS**: SQL Server database
- **ElastiCache**: Redis cache (tùy chọn)
- **CI/CD Pipeline**: GitLab → Code Pipeline → Code Build → Elastic Beanstalk
- **CloudWatch**: Monitoring và logging

---

## 🚀 Bước 1: Chuẩn Bị Tài Nguyên AWS

### 1.1. Tạo AWS Account và IAM User
1. Đăng nhập vào AWS Console
2. Tạo IAM user với quyền:
   - `AmazonEC2FullAccess`
   - `AmazonRDSFullAccess`
   - `AmazonS3FullAccess`
   - `AmazonElastiCacheFullAccess`
   - `AWSElasticBeanstalkFullAccess`
   - `AmazonRoute53FullAccess`
   - `CloudFrontFullAccess`
   - `AWSCodePipelineFullAccess`
   - `AWSCodeBuildDeveloperAccess`
   - `AmazonVPCFullAccess`
   - `CloudWatchFullAccess`
3. Tạo Access Key và Secret Key, lưu lại an toàn

### 1.2. Cài Đặt AWS CLI và EB CLI
```bash
# Cài AWS CLI
# Windows: Download từ https://aws.amazon.com/cli/
# Hoặc dùng: winget install Amazon.AWSCLI

# Cài Elastic Beanstalk CLI
pip install awsebcli

# Verify
aws --version
eb --version
```

---

## 🏗️ Bước 2: Tạo VPC và Network Infrastructure

### 2.1. Tạo VPC
1. Vào **VPC Dashboard** → **Create VPC**
2. Chọn **VPC and more**
3. Cấu hình:
   - **Name tag**: `mini-market-vpc`
   - **IPv4 CIDR**: `10.0.0.0/16`
   - **Number of Availability Zones**: 2 (ví dụ: us-east-1a, us-east-1b)
   - **Number of public subnets**: 2
   - **Number of private subnets**: 2
   - **NAT gateways**: 1 per AZ (hoặc 1 shared)
   - **VPC endpoints**: None (hoặc S3, DynamoDB nếu cần)
4. Click **Create VPC**

### 2.2. Tạo Internet Gateway (đã tự động tạo)
- Kiểm tra Internet Gateway đã được attach vào VPC

### 2.3. Kiểm Tra Subnets
- **Public Subnets**: Route table có route đến Internet Gateway (0.0.0.0/0 → igw-xxx)
- **Private Subnets**: Route table có route đến NAT Gateway (0.0.0.0/0 → nat-xxx)

---

## 💾 Bước 3: Tạo Amazon RDS (SQL Server)

### 3.1. Tạo DB Subnet Group
1. Vào **RDS Dashboard** → **Subnet groups** → **Create DB subnet group**
2. Cấu hình:
   - **Name**: `mini-market-db-subnet-group`
   - **VPC**: Chọn VPC vừa tạo
   - **Availability Zones**: Chọn 2 AZ
   - **Subnets**: Chọn 2 private subnets
3. Click **Create**

### 3.2. Tạo RDS Instance
1. Vào **RDS Dashboard** → **Databases** → **Create database**
2. Cấu hình:
   - **Engine**: Microsoft SQL Server
   - **Version**: SQL Server Express (hoặc Standard/Enterprise)
   - **Template**: Free tier (hoặc Production)
   - **DB instance identifier**: `mini-market-db`
   - **Master username**: `admin` (hoặc tên khác)
   - **Master password**: Tạo password mạnh, lưu lại
   - **Instance class**: `db.t3.micro` (free tier) hoặc `db.t3.small` (production)
   - **Storage**: 20 GB (tối thiểu)
   - **VPC**: Chọn VPC vừa tạo
   - **Subnet group**: Chọn subnet group vừa tạo
   - **Public access**: **No** (chỉ private)
   - **VPC security group**: Tạo mới `mini-market-db-sg`
   - **Database name**: `MiniMarketDB`
3. Click **Create database**

### 3.3. Cấu Hình Security Group cho RDS
1. Vào **EC2 Dashboard** → **Security Groups**
2. Chọn security group của RDS (`mini-market-db-sg`)
3. **Inbound rules** → **Edit inbound rules** → **Add rule**:
   - **Type**: MSSQL
   - **Port**: 1433
   - **Source**: Security group của Elastic Beanstalk (sẽ tạo sau)
   - **Description**: Allow from Elastic Beanstalk

---

## 📦 Bước 4: Tạo Amazon S3 Bucket cho Static Assets

### 4.1. Tạo S3 Bucket
1. Vào **S3 Dashboard** → **Create bucket**
2. Cấu hình:
   - **Bucket name**: `mini-market-static-assets` (phải unique globally)
   - **Region**: Chọn region của bạn
   - **Block Public Access**: Bỏ chọn (hoặc chỉ cho phép CloudFront)
   - **Versioning**: Enable (tùy chọn)
   - **Encryption**: Enable (SSE-S3 hoặc SSE-KMS)
3. Click **Create bucket**

### 4.2. Cấu Hình Bucket Policy
1. Vào bucket → **Permissions** → **Bucket policy**
2. Thêm policy để CloudFront có thể truy cập:
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "AllowCloudFrontAccess",
      "Effect": "Allow",
      "Principal": {
        "Service": "cloudfront.amazonaws.com"
      },
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::mini-market-static-assets/*",
      "Condition": {
        "StringEquals": {
          "AWS:SourceArn": "arn:aws:cloudfront::ACCOUNT_ID:distribution/DISTRIBUTION_ID"
        }
      }
    }
  ]
}
```
(Lưu ý: Cần thay ACCOUNT_ID và DISTRIBUTION_ID sau khi tạo CloudFront)

### 4.3. Upload Static Assets
1. Upload các file từ `WebShop/wwwroot/` lên S3:
   - CSS files
   - JS files
   - Images
   - Fonts
2. Có thể dùng AWS CLI:
```bash
aws s3 sync WebShop/wwwroot/ s3://mini-market-static-assets/wwwroot/ --exclude "*.cshtml"
```

---

## ☁️ Bước 5: Tạo CloudFront Distribution

### 5.1. Tạo Distribution
1. Vào **CloudFront Dashboard** → **Create distribution**
2. Cấu hình:
   - **Origin domain**: Chọn S3 bucket `mini-market-static-assets`
   - **Origin access**: Origin access control settings (recommended)
   - **Viewer protocol policy**: Redirect HTTP to HTTPS
   - **Allowed HTTP methods**: GET, HEAD, OPTIONS
   - **Cache policy**: CachingOptimized
   - **Price class**: Use all edge locations (hoặc chỉ North America/Europe)
   - **Alternate domain names (CNAMEs)**: `cdn.yourdomain.com` (nếu có)
   - **SSL certificate**: Default CloudFront certificate (hoặc custom nếu có domain)
3. Click **Create distribution**
4. Lưu lại **Distribution ID** và **Domain name** (ví dụ: `d1234567890.cloudfront.net`)

### 5.2. Tạo Origin Access Control (OAC)
1. Vào **CloudFront** → **Origin access** → **Create control setting**
2. Cấu hình:
   - **Name**: `mini-market-oac`
   - **Signing behavior**: Sign requests
   - **Origin type**: S3
3. Click **Create**
4. Update S3 bucket policy với OAC ARN

---

## 🛡️ Bước 6: Cấu Hình Amazon WAF

### 6.1. Tạo Web ACL
1. Vào **WAF & Shield Dashboard** → **Web ACLs** → **Create web ACL**
2. Cấu hình:
   - **Name**: `mini-market-waf`
   - **Resource type**: CloudFront distribution
   - **CloudFront distribution**: Chọn distribution vừa tạo
   - **Default action**: Allow
3. Thêm rules:
   - **AWS Managed Rules - Core rule set**
   - **AWS Managed Rules - Known bad inputs**
   - **AWS Managed Rules - SQL database**
   - **Rate-based rule**: Giới hạn 2000 requests/5 phút từ 1 IP
4. Click **Create**

---

## 🌐 Bước 7: Cấu Hình Route 53 (DNS)

### 7.1. Tạo Hosted Zone (nếu có domain)
1. Vào **Route 53 Dashboard** → **Hosted zones** → **Create hosted zone**
2. Cấu hình:
   - **Domain name**: `yourdomain.com`
   - **Type**: Public hosted zone
3. Click **Create hosted zone**

### 7.2. Tạo DNS Records
1. **A Record cho CloudFront**:
   - **Name**: `www` (hoặc `@`)
   - **Type**: A - IPv4 address
   - **Alias**: Yes
   - **Route traffic to**: CloudFront distribution
   - **Distribution**: Chọn CloudFront distribution
   - **Evaluate target health**: No

2. **CNAME cho CDN** (nếu cần):
   - **Name**: `cdn`
   - **Type**: CNAME
   - **Value**: `d1234567890.cloudfront.net`

---

## 🚢 Bước 8: Chuẩn Bị Ứng Dụng .NET cho Elastic Beanstalk

### 8.1. Tạo File `.ebextensions/01-environment.config`
Tạo thư mục `.ebextensions` trong project root và file config:
```yaml
option_settings:
  aws:elasticbeanstalk:application:environment:
    ASPNETCORE_ENVIRONMENT: Production
    ConnectionStrings__DefaultConnection: "Server=YOUR_RDS_ENDPOINT;Database=MiniMarketDB;User Id=admin;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  aws:elasticbeanstalk:container:dotnet:apppool:
    Enable 32-bit Applications: false
```

### 8.2. Tạo File `.ebextensions/02-nginx.config` (nếu cần)
```yaml
files:
  "/etc/nginx/conf.d/proxy.conf":
    mode: "000644"
    owner: root
    group: root
    content: |
      client_max_body_size 10M;
```

### 8.3. Tạo File `Dockerrun.aws.json` (nếu dùng Docker)
```json
{
  "AWSEBDockerrunVersion": "1",
  "Image": {
    "Name": "YOUR_ECR_IMAGE_URI",
    "Update": "true"
  },
  "Ports": [
    {
      "ContainerPort": "8080"
    }
  ],
  "Environment": [
    {
      "Name": "ASPNETCORE_ENVIRONMENT",
      "Value": "Production"
    }
  ]
}
```

### 8.4. Update `appsettings.Production.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_RDS_ENDPOINT;Database=MiniMarketDB;User Id=admin;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 8.5. Update Code để Sử Dụng S3/CloudFront cho Static Assets
Cần update `Program.cs` hoặc `Startup.cs` để:
- Serve static files từ CloudFront URL
- Hoặc cấu hình `StaticFileOptions` để point đến CloudFront

---

## 🎯 Bước 9: Tạo Elastic Beanstalk Application

### 9.1. Tạo Application
1. Vào **Elastic Beanstalk Dashboard** → **Create application**
2. Cấu hình:
   - **Application name**: `mini-market`
   - **Platform**: .NET
   - **Platform branch**: .NET 8 running on 64bit Amazon Linux 2023
   - **Platform version**: Latest
   - **Application code**: Upload your code (zip file)
3. Click **Configure more options**:
   - **Presets**: Custom configuration
   - **Capacity**:
     - **Environment type**: Load balanced
     - **Load balancer type**: Application Load Balancer
     - **Min instances**: 1
     - **Max instances**: 4
     - **Instance type**: t3.small (hoặc t3.medium)
   - **Load balancer**:
     - **Listeners**: HTTP (80) → HTTPS (443)
     - **Processes**: Default process (port 80)
     - **Health check**: `/health` hoặc `/`
   - **Rolling updates and deployments**:
     - **Deployment policy**: Rolling
     - **Batch size**: 1
   - **Security**:
     - **EC2 key pair**: Tạo mới hoặc chọn existing
     - **IAM instance profile**: aws-elasticbeanstalk-ec2-role
     - **Service role**: aws-elasticbeanstalk-service-role
   - **VPC**:
     - **VPC**: Chọn VPC vừa tạo
     - **Public IP**: No (đặt trong private subnet)
     - **Subnets**: Chọn private subnets
     - **Load balancer subnets**: Chọn public subnets
4. Click **Create environment**

### 9.2. Cấu Hình Environment Variables
1. Vào **Configuration** → **Software** → **Environment properties**
2. Thêm:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `ConnectionStrings__DefaultConnection`: Connection string đến RDS
   - `CloudFrontUrl`: URL của CloudFront distribution

### 9.3. Cấu Hình Security Group
1. Vào **EC2 Dashboard** → **Security Groups**
2. Tìm security group của Elastic Beanstalk
3. **Inbound rules** → **Add rule**:
   - **Type**: HTTP
   - **Port**: 80
   - **Source**: 0.0.0.0/0 (hoặc chỉ ALB security group)
   - **Description**: Allow HTTP from ALB

---

## 🔄 Bước 10: Thiết Lập CI/CD Pipeline

### 10.1. Tạo GitLab Repository
1. Tạo repository trên GitLab
2. Push code lên GitLab:
```bash
git remote add gitlab https://gitlab.com/yourusername/mini-market.git
git push -u gitlab main
```

### 10.2. Tạo CodeBuild Project
1. Vào **CodeBuild Dashboard** → **Create build project**
2. Cấu hình:
   - **Project name**: `mini-market-build`
   - **Source provider**: GitLab (hoặc GitHub)
   - **Repository**: Chọn GitLab repository
   - **Environment**:
     - **Operating system**: Ubuntu
     - **Runtime**: Standard
     - **Image**: aws/codebuild/standard:7.0
     - **Image version**: Latest
     - **Environment type**: Linux
     - **Privileged**: Check (nếu cần Docker)
   - **Buildspec**: Sử dụng file `buildspec.yml` trong repo
3. Click **Create build project**

### 10.3. Tạo File `buildspec.yml` trong Project Root
```yaml
version: 0.2
phases:
  pre_build:
    commands:
      - echo Logging in to Amazon ECR...
      - aws --version
      - dotnet --version
  build:
    commands:
      - echo Build started on `date`
      - echo Building the .NET application...
      - cd WebShop
      - dotnet restore
      - dotnet build -c Release
      - dotnet publish -c Release -o ./publish
  post_build:
    commands:
      - echo Build completed on `date`
artifacts:
  files:
    - '**/*'
  base-directory: 'WebShop/publish'
```

### 10.4. Tạo CodePipeline
1. Vào **CodePipeline Dashboard** → **Create pipeline**
2. Cấu hình:
   - **Pipeline name**: `mini-market-pipeline`
   - **Source stage**:
     - **Source provider**: GitLab (hoặc GitHub)
     - **Repository**: Chọn repository
     - **Branch**: main
     - **Change detection options**: CloudWatch Events
   - **Build stage**:
     - **Build provider**: AWS CodeBuild
     - **Project name**: `mini-market-build`
   - **Deploy stage**:
     - **Deploy provider**: AWS Elastic Beanstalk
     - **Application name**: `mini-market`
     - **Environment name**: Chọn environment vừa tạo
3. Click **Create pipeline**

---

## 📊 Bước 11: Cấu Hình CloudWatch Monitoring

### 11.1. Tạo CloudWatch Dashboard
1. Vào **CloudWatch Dashboard** → **Create dashboard**
2. Thêm widgets:
   - **EC2 Metrics**: CPU utilization, Network in/out
   - **RDS Metrics**: CPU, Database connections, Read/Write IOPS
   - **Elastic Beanstalk**: Request count, 4xx/5xx errors
   - **Application Load Balancer**: Request count, Target response time

### 11.2. Tạo CloudWatch Alarms
1. **High CPU Usage**:
   - **Metric**: EC2 CPU Utilization
   - **Threshold**: > 80% for 5 minutes
   - **Action**: SNS notification

2. **Database Connection Issues**:
   - **Metric**: RDS Database Connections
   - **Threshold**: > 80% of max connections
   - **Action**: SNS notification

3. **High Error Rate**:
   - **Metric**: Elastic Beanstalk 5xx errors
   - **Threshold**: > 10 errors in 5 minutes
   - **Action**: SNS notification

---

## 🔧 Bước 12: Cấu Hình ElastiCache (Tùy Chọn)

### 12.1. Tạo ElastiCache Subnet Group
1. Vào **ElastiCache Dashboard** → **Subnet groups** → **Create subnet group**
2. Cấu hình:
   - **Name**: `mini-market-cache-subnet-group`
   - **VPC**: Chọn VPC
   - **Subnets**: Chọn private subnets

### 12.2. Tạo Redis Cluster
1. Vào **ElastiCache Dashboard** → **Redis clusters** → **Create Redis cluster**
2. Cấu hình:
   - **Name**: `mini-market-redis`
   - **Engine version**: Latest
   - **Node type**: cache.t3.micro (hoặc cache.t3.small)
   - **Number of replicas**: 0 (hoặc 1-2)
   - **Subnet group**: Chọn subnet group vừa tạo
   - **Security groups**: Tạo mới, allow port 6379 từ Elastic Beanstalk SG
3. Click **Create**

---

## ✅ Bước 13: Kiểm Tra và Testing

### 13.1. Kiểm Tra Kết Nối
1. Test connection từ Elastic Beanstalk đến RDS
2. Test CloudFront distribution
3. Test WAF rules
4. Test Route 53 DNS resolution

### 13.2. Deploy Application
1. Trigger CodePipeline hoặc deploy manual qua EB CLI:
```bash
eb init -p .NET-8-on-64bit-Amazon-Linux-2023 mini-market
eb create mini-market-prod --vpc.id vpc-xxx --vpc.subnets subnet-xxx,subnet-yyy --vpc.elbsubnets subnet-xxx,subnet-yyy
eb deploy
```

### 13.3. Chạy Database Migrations
1. SSH vào EC2 instance của Elastic Beanstalk
2. Hoặc chạy migrations trong `Program.cs` (đã có sẵn)
3. Verify database schema

---

## 🔐 Bước 14: Security Best Practices

1. **SSL/TLS**: Enable HTTPS trên ALB và CloudFront
2. **Secrets Management**: Sử dụng AWS Secrets Manager cho passwords
3. **IAM Roles**: Sử dụng IAM roles thay vì access keys
4. **Security Groups**: Chỉ mở ports cần thiết
5. **WAF Rules**: Cấu hình rules để chặn SQL injection, XSS
6. **Backup**: Enable automated backups cho RDS
7. **Monitoring**: Enable CloudTrail và VPC Flow Logs

---

## 📝 Checklist Trước Khi Deploy

- [ ] VPC và subnets đã được tạo
- [ ] RDS instance đã được tạo và accessible từ private subnet
- [ ] S3 bucket đã được tạo và static assets đã upload
- [ ] CloudFront distribution đã được tạo và point đến S3
- [ ] WAF web ACL đã được attach vào CloudFront
- [ ] Route 53 DNS records đã được cấu hình
- [ ] Elastic Beanstalk environment đã được tạo
- [ ] Security groups đã được cấu hình đúng
- [ ] Environment variables đã được set
- [ ] CI/CD pipeline đã được thiết lập
- [ ] CloudWatch monitoring đã được cấu hình
- [ ] Code đã được update để sử dụng CloudFront cho static assets
- [ ] Connection string đã được cấu hình đúng
- [ ] SSL certificates đã được cấu hình (nếu có custom domain)

---

## 💰 Ước Tính Chi Phí (Tham Khảo)

- **RDS (db.t3.small)**: ~$15-20/tháng
- **Elastic Beanstalk (t3.small, 2 instances)**: ~$30-40/tháng
- **Application Load Balancer**: ~$16/tháng
- **CloudFront**: ~$0.085/GB data transfer
- **S3**: ~$0.023/GB storage
- **Route 53**: ~$0.50/hosted zone + $0.40/million queries
- **WAF**: ~$5/tháng + $1/million requests
- **ElastiCache (cache.t3.micro)**: ~$12/tháng
- **NAT Gateway**: ~$32/tháng + data transfer
- **CodePipeline**: ~$1/tháng + $0.002/active minute
- **CodeBuild**: ~$0.005/build minute

**Tổng ước tính**: ~$150-200/tháng (tùy traffic và usage)

---

## 📚 Tài Liệu Tham Khảo

- [AWS Elastic Beanstalk .NET Guide](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_NET.html)
- [AWS RDS SQL Server Guide](https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_SQLServer.html)
- [CloudFront Documentation](https://docs.aws.amazon.com/cloudfront/)
- [CodePipeline User Guide](https://docs.aws.amazon.com/codepipeline/)
- [VPC Best Practices](https://docs.aws.amazon.com/vpc/latest/userguide/vpc-security-best-practices.html)

---

## 🆘 Troubleshooting

### Lỗi Connection Timeout đến RDS
- Kiểm tra security groups
- Kiểm tra RDS endpoint và port
- Kiểm tra VPC routing

### Static Assets không load từ CloudFront
- Kiểm tra S3 bucket policy
- Kiểm tra CloudFront origin configuration
- Kiểm tra CORS settings

### Elastic Beanstalk deployment failed
- Kiểm tra CloudWatch logs
- Kiểm tra buildspec.yml
- Kiểm tra environment variables

---

**Lưu ý**: Đây là guide tổng quan. Mỗi bước có thể cần điều chỉnh dựa trên requirements cụ thể của bạn. Nên test trên môi trường staging trước khi deploy production.

