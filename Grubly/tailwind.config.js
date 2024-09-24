/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Pages/**/*.cshtml',
    './Views/**/*.cshtml',
    './Areas/Identity/Pages/**/*.cshtml'
  ],
  theme: {
    container: {
      center: true,
      padding: {
        DEFAULT: '1rem',
        sm: '2rem',
        lg: '4rem',
        xl: '5rem',
        '2xl': '6rem',
      }
    },
    extend: {
      colors: {
        primary: {
          DEFAULT: '#38BC64',
          50:  '#E9F6EE',
          100: '#C7EBD3',
          200: '#A3DFB7',
          300: '#7FD39B',
          400: '#5CC880',
          500: '#38BC64',
          600: '#2E9D54',
          700: '#247D43',
          800: '#1A5E33',
          900: '#103F22',
        },
        secondary: {
          DEFAULT: '#F9A825',
          50:  '#FFF8E1',
          100: '#FFECB3',
          200: '#FFE082',
          300: '#FFD54F',
          400: '#FFCA28',
          500: '#F9A825',
          600: '#F57F17',
          700: '#EF6C00',
          800: '#E65100',
          900: '#BF360C',
        },
        muted: {
          DEFAULT: '#1f2937',
          50:  '#f5f7fa',
          100: '#e4e7eb',
          200: '#cbd5e1',
          300: '#a0aec0',
          400: '#718096',
          500: '#4a5568',
          600: '#2d3748',
          700: '#24303f',
          800: '#1f2937',
          900: '#171f29',
        },
      }
    },
  },
  plugins: [],
};