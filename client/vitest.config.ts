import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'happy-dom',
    globals: true,
  },
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:5284',
                changeOrigin: true,
            }
        }
    },
  resolve: {
    alias: {
      '@core': path.resolve(__dirname, './src/core'),
      '@utilities': path.resolve(__dirname, './src/utilities'),
      '@components': path.resolve(__dirname, './src/components')
    }
  }
})
