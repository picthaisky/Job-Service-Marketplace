/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: '#2563EB',
        secondary: '#10B981',
        accent: '#8B5CF6',
        warning: '#F59E0B',
        danger: '#EF4444',
        info: '#06B6D4',
      },
    },
  },
  plugins: [],
}

