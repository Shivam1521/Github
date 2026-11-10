import { createTheme } from "@mui/material/styles";

export const panasonicTheme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: "#0056A8",
      contrastText: "#fff",
    },
    background: {
      default: "#F4F6F8",
      paper: "#ffffff",
    },
    text: {
      primary: "#1A202C", // dark gray for main text
      secondary: "#4A5568", // softer gray for secondary text
    },
  },
  components: {
    MuiAppBar: {
      styleOverrides: {
        root: {
          borderBottom: "1px solid #E2E8F0",
          backgroundColor: "#ffffff",
          color: "#1A202C",
        },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          background: "#ffffff",
          color: "#1A202C",
          borderRight: "1px solid #E2E8F0",
        },
      },
    },
    MuiListItemButton: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          margin: "8px 14px",
          padding: "12px 18px",
          fontSize: "0.95rem",
          fontWeight: 500,
          letterSpacing: "0.3px",
          color: "#f0f0f0",
          transition: "all 0.3s ease",
          backdropFilter: "blur(6px)",

          "& .MuiListItemIcon-root": {
            color: "inherit",
            minWidth: 32,
            marginRight: 14,
            transition: "color 0.25s ease",
          },

          "&:hover": {
            backgroundColor: "rgba(255,255,255,0.15)", // frosted hover
            color: "#ffffff",
            transform: "translateX(6px) scale(1.05)",
            boxShadow: "0 8px 20px rgba(0,0,0,0.25)",
          },

          "&.active": {
            background: "rgba(255,255,255,0.15)",
            color: "#ffffff",
            fontWeight: 700,
            borderLeft: "5px solidrgb(49, 56, 63)", // bright blue accent
            boxShadow: "inset 5px 0 12px rgba(59, 68, 76, 0.35)",
            transform: "translateX(3px) scale(1.02)",
          },
        },
      },
    },
  },
});
