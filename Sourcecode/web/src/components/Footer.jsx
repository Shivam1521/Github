import { Box, Typography } from "@mui/material";
import CopyrightIcon from "@mui/icons-material/Copyright";
import SecurityIcon from "@mui/icons-material/Security";

const Footer = () => {
  return (
    <Box
      component="footer"
      sx={{
        position: "fixed",
        bottom: 0,
        width: "100%",
        py: 1,
        px: 3,
        zIndex: 1300,
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        backgroundColor: "#1e1e1e",
        borderTop: "1px solid #444",
        color: "#aaa",
        fontSize: "0.9rem",
        transition: "left 0.3s ease, width 0.3s ease",
      }}
    >
      <Typography
        variant="body2"
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 0.5,
        }}
      >
        <CopyrightIcon fontSize="small" />
        {new Date().getFullYear()} Smart Scan Application
      </Typography>
      <Typography
        variant="body2"
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 0.5,
        }}
      >
        <SecurityIcon fontSize="small" />
        Developed by
        <strong>
          <a
            style={{ color: " #0658a0" }}
            href="https://holostik.com/"
            target="_blank"
          >
            HOLOSTIK INDIA LTD
          </a>
        </strong>
      </Typography>
    </Box>
  );
};

export default Footer;
