FROM node:22-alpine AS build
RUN corepack enable && corepack prepare pnpm@10.6.2 --activate
WORKDIR /app
# Copy in two stages for cache optimisation
COPY package.json pnpm-lock.yaml* ./
RUN pnpm install --frozen-lockfile
COPY . .
RUN pnpm run build

FROM nginx:alpine AS final
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
