import React, { useEffect, useState } from "react";
import {
  AppBar,
  Toolbar,
  Typography,
  IconButton,
  Box,
  MenuItem,
  Menu,
  useTheme,
  Avatar,
  Divider,
  ListItemIcon,
  ListItemText,
  FormControl,
  Select,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import { AccountCircle, Factory, LockReset, Logout } from "@mui/icons-material";
import { Link, useNavigate } from "react-router-dom";
import logo from "../assets/images/logo.png";
import { fetchUnits } from "../store/commonSlice";
import { useDispatch, useSelector } from "react-redux";
import { setUnit } from "../store/unitSlice";
import unitAccessHook from "./UnitAccessHook";

const Navbar = ({ toggleSidebar }) => {
  const navigate = useNavigate();
  const { isSuperAdmin, unitId } = unitAccessHook();
  const { units } = useSelector((state) => state.lists);
  const [anchorEl, setAnchorEl] = useState(null);
  const dispatch = useDispatch();

  const handleMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  useEffect(() => {
    dispatch(fetchUnits());
  }, []);

  const handleClose = () => {
    sessionStorage.clear();
    navigate("/login");
  };

  const name = sessionStorage.getItem("userName");

  const onUnitChange = (e) => {
    dispatch(setUnit(e.target.value));
  };

  return (
    <AppBar
      position="fixed"
      elevation={0}
      sx={{
        zIndex: 1201,
        background: "linear-gradient(90deg, #121721, #1E2733)",
        borderBottom: "1px solid #2D3748",
        px: 3,
      }}
    >
      <Toolbar
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
          <IconButton
            edge="start"
            onClick={toggleSidebar}
            sx={{ color: "#fff" }}
          >
            <MenuIcon />
          </IconButton>
          <img src={logo} alt="Logo" height={36} />
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
          <FormControl
            disabled={!isSuperAdmin}
            size="small"
            className="unit-select"
            sx={{ bgcolor: "rgba(255, 255, 255, 1)", borderRadius: "8px" }}
          >
            {units.length > 0 && (
              <Select
                value={unitId || ""}
                onChange={onUnitChange}
                renderValue={(selected) => {
                  const unit = units.find(
                    (u) => u.value === selected.toString(),
                  );
                  return (
                    <Box display="flex" alignItems="center" gap={1.5}>
                      <Factory
                        sx={{ color: "rgb(255, 12, 12)", fontSize: 20 }}
                      />
                      <span>{unit?.label}</span>
                    </Box>
                  );
                }}
              >
                {units.map((unit) => (
                  <MenuItem key={unit.value} value={unit.value}>
                    <Box display="flex" alignItems="center" gap={1.5}>
                      <Factory sx={{ color: "#475569", fontSize: 18 }} />
                      {unit.label}
                    </Box>
                  </MenuItem>
                ))}
              </Select>
            )}
          </FormControl>
          <IconButton onClick={handleMenu} sx={{ p: 0 }}>
            <Avatar
              variant="circular"
              sx={{
                bgcolor: "rgb(182, 68, 68)",
                color: "#fff",
                width: 40,
                height: 40,
                fontWeight: "700",
                fontSize: "1rem",
                borderRadius: "50%",
                boxShadow: "0 3px 6px rgba(0,0,0,0.2)",
                transform: "scale(1)",
                transition: "all 0.3s ease",
                "&:hover": {
                  bgcolor: "rgba(173, 38, 38, 0.85)",
                  transform: "scale(1.08)",
                  boxShadow: "0 4px 10px rgba(0,0,0,0.25)",
                },
              }}
            ></Avatar>
          </IconButton>

          <Menu
            anchorEl={anchorEl}
            anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
            transformOrigin={{ vertical: "top", horizontal: "right" }}
            open={Boolean(anchorEl)}
            onClose={() => setAnchorEl(null)}
            PaperProps={{
              elevation: 3,
              sx: {
                mt: 1,
                borderRadius: "12px",
                minWidth: 180,
                bgcolor: "#fff",
                boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
                overflow: "hidden",
              },
            }}
          >
            <MenuItem disabled sx={{ px: 2 }}>
              <Typography
                variant="body2"
                sx={{ fontWeight: 600, color: "#000", letterSpacing: 0.3 }}
              >
                Welcome, {name}
              </Typography>
            </MenuItem>
            <Divider />
            <MenuItem
              component={Link}
              to="/change-password"
              sx={{
                color: "#333",
                textDecoration: "none",
                "&:hover": {
                  bgcolor: "rgba(182, 68, 68, 0.08)",
                  color: "#333",
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 32, color: "rgb(182, 68, 68)" }}>
                <LockReset fontSize="small" />
              </ListItemIcon>
              <ListItemText primary="Change Password" />
            </MenuItem>
            <Divider />
            <MenuItem
              onClick={handleClose}
              sx={{
                px: 2,
                color: "#333",
                fontWeight: 500,
                "&:hover": {
                  bgcolor: "rgba(182, 68, 68, 0.08)",
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 32, color: "rgb(182, 68, 68)" }}>
                <Logout fontSize="small" />
              </ListItemIcon>
              Logout
            </MenuItem>
          </Menu>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
