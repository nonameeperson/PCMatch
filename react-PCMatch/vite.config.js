import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5049/', // Порт, на якому працює ваш .NET Swagger
        changeOrigin: true,
        secure: false,
      }
    }
  }
});