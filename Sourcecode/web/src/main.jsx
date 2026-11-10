import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.jsx";
import { BrowserRouter } from "react-router-dom";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import { panasonicTheme } from "./helper/theme.js";
import { Provider } from "react-redux";
import { store } from "./store/store.js";
import { PermissionProvider } from "./helper/permissionContext.jsx";

const theme = createTheme();
createRoot(document.getElementById("root")).render(
  <Provider store={store}>
    <BrowserRouter>
      <ThemeProvider theme={panasonicTheme}>
        <CssBaseline />
        <PermissionProvider>
          <App />
        </PermissionProvider>
      </ThemeProvider>
    </BrowserRouter>
  </Provider>
);
