# Sử dụng Node.js LTS để build Angular
FROM node:20 AS build

# Đặt thư mục làm việc trong container
WORKDIR /app

# Sao chép package.json và package-lock.json
COPY package.json package-lock.json ./

# Cài đặt dependencies
RUN npm install --legacy-peer-deps

# Cài đặt Angular CLI toàn cục
RUN npm install -g @angular/cli

# Sao chép toàn bộ mã nguồn vào container
COPY . .

# Sao chép chứng chỉ SSL vào container
COPY localhost.crt /etc/ssl/certs/localhost.crt
COPY localhost.key /etc/ssl/private/localhost.key

# Mở cổng Angular dev server
EXPOSE 4200

# Chạy Angular (không cần chỉ định SSL vì đã có trong angular.json)
CMD ["npx", "ng", "serve", "--host", "0.0.0.0", "--port", "4200"]
