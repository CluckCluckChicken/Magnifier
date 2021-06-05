module.exports = {
  future: {
    // removeDeprecatedGapUtilities: true,
    // purgeLayersByDefault: true,
  },
  purge: [],
  theme: {
      extend: {
          colors: {
              black: "#000000",
              pureWhite: "#ffffff",
              white: "#ecf0f1",
              whiteHover: "#bdc3c7",
              red: "#e74c3c",
              blue: "#3498db",
              primary: "#1abc9c",
              primaryHover: "#16a085",
              grey: "#7f8c8d",
              darkGrey: "#323232",
              midnight: "#2c3e50",
          },
          fontFamily: {
              dosis: ["Dosis"],
              didactGothic: ["DidactGothic"],
          },
          outline: ['active'],
          animation: {
              rotate: 'spin 0.5s ease-in-out infinite',
          }
      },
  },
  variants: {},
  plugins: [],
}
