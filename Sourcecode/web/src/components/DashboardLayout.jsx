// components/DashboardLayout.jsx
import React, { useEffect, useState } from "react";
import { Box, Toolbar, useMediaQuery } from "@mui/material";
import { Outlet } from "react-router-dom";
import Sidebar from "./Sidebar";
import Navbar from "./Navbar";
import { useTheme } from "@emotion/react";
import Footer from "./Footer";

const DashboardLayout = () => {
  const [sidebarOpen, setSidebarOpen] = useState(true);

  const toggleSidebar = () => {
    setSidebarOpen((prev) => !prev);
  };
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

  useEffect(() => {
    if (isMobile) {
      setSidebarOpen(false);
    }
  }, [isMobile]);

  return (
    <Box sx={{ display: "flex", bgcolor: "#f0f2f5" }}>
      <Navbar toggleSidebar={toggleSidebar} />
      <Sidebar open={sidebarOpen} />

      <Box
        component="main"
        sx={(theme) => ({
          flexGrow: 1,
          px: 3,
          width: "100%",
          py: 2,
          transition: "margin 0.3s ease",
          ml: sidebarOpen ? "280px" : "0px",
          bgcolor: "#f0f2f5",
          color: "#333",
          overflowX: "hidden",
          mb: 4,
          height: "100%",
          borderTopLeftRadius: sidebarOpen ? theme.shape.borderRadius * 2 : 0,
          // boxShadow: sidebarOpen ? "inset 6px 0 10px rgba(0,0,0,0.05)" : "none",
        })}
      >
        <Toolbar />
        <Outlet />
      </Box>
      <Footer />
    </Box>
  );
};

export default DashboardLayout;
