import React, { useEffect, useState } from "react";
import {
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Box,
  Toolbar,
  Collapse,
} from "@mui/material";
import DashboardIcon from "@mui/icons-material/Dashboard";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import ExpandLess from "@mui/icons-material/ExpandLess";
import ExpandMore from "@mui/icons-material/ExpandMore";
import PrintIcon from "@mui/icons-material/Print";
import InsertChartIcon from "@mui/icons-material/InsertChart";
import SettingsIcon from "@mui/icons-material/Settings";
import { NavLink, useLocation } from "react-router-dom";
import { usePermissions } from "../helper/permissionContext";
import CasinoSharpIcon from "@mui/icons-material/CasinoSharp";
import PaymentIcon from "@mui/icons-material/Payment";
const Sidebar = ({ open }) => {
  const [openSubmenu, setOpenSubmenu] = useState(null);
  const location = useLocation();
  const currentPath = location.pathname;
  const { permissions, loading } = usePermissions();

  const handleToggle = (label) => {
    setOpenSubmenu((prev) => (prev === label ? null : label));
  };

  const items = !loading
    ? permissions?.map((groupItem) => {
        let icon;
        switch (groupItem.group.toLowerCase()) {
          case "dashboard":
            icon = <DashboardIcon />;
            break;
          case "masters":
            icon = <ManageAccountsIcon />;
            break;
          case "printing":
            icon = <PrintIcon />;
            break;
          case "reports":
            icon = <InsertChartIcon />;
            break;
          case "transactions":
            icon = <PaymentIcon />;
            break;
          default:
            icon = <SettingsIcon />;
        }
        return {
          label: groupItem.group,
          path: groupItem.path || "",
          icon,
          children: Array.isArray(groupItem.children)
            ? groupItem.children
                .filter((child) => child.isView)
                .map((child) => ({
                  label: child.pageName,
                  path: "/" + child.path,
                  ...child,
                }))
            : null,
        };
      })
    : [];
  const isParentActive = (parent) => currentPath === parent.path;

  return (
    <Drawer
      variant="persistent"
      anchor="left"
      open={open}
      sx={{
        flexShrink: 0,
        "& .MuiDrawer-paper": {
          width: 280,
          boxSizing: "border-box",
          background: "rgba(88, 88, 88, 0.98)",
          color: "#ffffff",
        },
      }}
    >
      <Toolbar />
      <Box sx={{ overflow: "auto", pr: 2, pb: 5 }}>
        <List>
          {items.map(({ label, path, icon, children }) => {
            const isOpen = openSubmenu === label;
            const isActive = isParentActive({ path });

            return (
              <React.Fragment key={label}>
                <ListItemButton
                  component={path ? NavLink : "div"}
                  to={path || undefined}
                  className={`list-item ${isActive ? "active" : ""}`}
                  onClick={() => children && handleToggle(label)}
                  sx={{
                    borderRadius: 2,
                    my: 1,

                    transition: "all 0.3s ease",
                  }}
                >
                  {icon && (
                    <ListItemIcon sx={{ color: "inherit" }}>
                      {icon}
                    </ListItemIcon>
                  )}
                  <ListItemText primary={label} />
                  {children ? isOpen ? <ExpandLess /> : <ExpandMore /> : null}
                </ListItemButton>

                {children && (
                  <Collapse in={isOpen} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                      {children.map((child) => (
                        <ListItemButton
                          key={child.label}
                          component={NavLink}
                          to={child.path}
                          className={`list-item ${
                            currentPath === child.path ? "active" : ""
                          }`}
                          sx={{
                            pl: 4,
                            borderRadius: 2,
                            my: 0.5,
                          }}
                        >
                          <ListItemText primary={child.label} />
                        </ListItemButton>
                      ))}
                    </List>
                  </Collapse>
                )}
              </React.Fragment>
            );
          })}
        </List>
      </Box>
    </Drawer>
  );
};

export default Sidebar;
