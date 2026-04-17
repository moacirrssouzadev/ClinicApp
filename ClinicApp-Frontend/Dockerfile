# Stage 1: Build
FROM node:20-alpine AS build

WORKDIR /app

# Security: Usar npm ci para instalações reprodutíveis
COPY package*.json ./
RUN npm ci

# Copiar o código fonte e gerar o build
COPY . .
RUN npm run build

# Stage 2: Production (Serving with Nginx)
FROM nginx:alpine

# Copiar os artefatos do build para o diretório do Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copiar configuração customizada do Nginx com headers de segurança
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Healthcheck para garantir que o serviço está respondendo
HEALTHCHECK --interval=30s --timeout=3s \
  CMD wget --quiet --tries=1 --spider http://localhost:80 || exit 1

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
