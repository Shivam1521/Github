import React, { useState } from "react";
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  InputAdornment,
  IconButton,
  LinearProgress,
  Paper
} from "@mui/material";
import { Visibility, VisibilityOff, ArrowBack } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";
import ChangePasswordService from "../services/ChangePasswordService";
import toast from "react-hot-toast";
import security from "../assets/security.gif";

const labelStyle = {
  fontWeight: 600,
  fontSize: "0.9rem",
  mb: 1,
  display: "block",
  color: "#333"
};

const textFieldStyles = {
  "& .MuiOutlinedInput-root": {
    borderRadius: 2,
    background: "#fff",
    boxShadow: "0 2px 6px rgba(0,0,0,0.1)",
    transition: "0.3s",
    "&:hover": {
      boxShadow: "0 4px 12px rgba(0,0,0,0.15)"
    },
    "&.Mui-focused": {
      boxShadow: "0 0 0 2px rgba(182,68,68,0.3)"
    }
  },
  "& .MuiFormHelperText-root": {
    color: "red",
    fontWeight: 500
  }
};

export const ChangePassword = () => {
  const [formData, setFormData] = useState({
    oldPassword: "",
    newPassword: ""
  });
  const [errors, setErrors] = useState({});
  const [showPassword, setShowPassword] = useState(false);
  const [strength, setStrength] = useState(0);
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    setErrors((prev) => ({ ...prev, [name]: "" }));
    if (name === "newPassword") setStrength(checkPasswordStrength(value));
  };

  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{4,}$/;
  const checkPasswordStrength = (password) => {
    let score = 0;
    if (password.length > 5) score += 25;
    if (/[A-Z]/.test(password)) score += 25;
    if (/[0-9]/.test(password)) score += 25;
    if (/[^A-Za-z0-9]/.test(password)) score += 25;
    return score;
  };

  const validate = () => {
    const newErrors = {};
    if (!formData.oldPassword)
      newErrors.oldPassword = "Current password is required";
    if (!formData.newPassword) {
      newErrors.newPassword = "New password is required";
    } else if (!passwordRegex.test(formData.newPassword)) {
      newErrors.newPassword =
        "Password should be minimum 4 characters, 1 uppercase & 1 lowercase character, and 1 number.";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const userId = sessionStorage.getItem("userId");
  const changePasswordService = new ChangePasswordService();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;
    setLoading(true);
    const payload = {
      oldPassword: formData.oldPassword,
      newPassword: formData.newPassword,
      userId
    };
    try {
      const response = await changePasswordService.ChangePassword(payload);
      if (response.responseCode === "200") {
        toast.success(response.responseMessage + ", Redirecting to login...");
        setFormData({ oldPassword: "", newPassword: "" });
        setStrength(0);
        setErrors({});
        setTimeout(() => {
          sessionStorage.clear();
          navigate("/login");
        }, 2000);
      } else {
        toast.error(response.responseMessage);
      }
    } catch (error) {
      toast.error(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        minHeight: "80vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center"
        // background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
      }}
    >
      {loading ? (
        <Paper
          elevation={6}
          sx={{
            p: 4,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            borderRadius: 3,
            bgcolor: "rgba(255,255,255,0.9)"
          }}
        >
          <img src={security} alt="Loading..." style={{ width: 120 }} />
          <Typography
            variant="h6"
            sx={{ mt: 2, color: "#764ba2", fontWeight: 600 }}
          >
            Updating password securely…
          </Typography>
        </Paper>
      ) : (
        <Card
          sx={{
            maxWidth: 500,
            width: "100%",
            borderRadius: 4,
            boxShadow: "0 20px 40px rgba(0,0,0,0.25)",
            overflow: "hidden",
            background: "rgba(255,255,255,0.95)"
          }}
        >
          <CardContent sx={{ p: 5 }}>
            <Box sx={{ position: "relative", mb: 4 }}>
              <IconButton
                onClick={() => navigate("/dashboard")}
                sx={{
                  position: "absolute",
                  left: 0,
                  top: "50%",
                  transform: "translateY(-50%)",
                  bgcolor: "#764ba2",
                  color: "#fff",
                  "&:hover": { bgcolor: "#667eea" }
                }}
              >
                <ArrowBack />
              </IconButton>
              <Typography
                variant="h5"
                fontWeight={700}
                sx={{ textAlign: "center", color: "#333" }}
              >
                Change Password
              </Typography>
            </Box>

            <Box
              component="form"
              onSubmit={handleSubmit}
              sx={{ display: "flex", flexDirection: "column", gap: 3 }}
            >
              <Box>
                <Typography sx={labelStyle}>Current Password</Typography>
                <TextField
                  size="small"
                  type={"password"}
                  name="oldPassword"
                  fullWidth
                  value={formData.oldPassword}
                  onChange={handleChange}
                  error={!!errors.oldPassword}
                  helperText={errors.oldPassword}
                  sx={textFieldStyles}
                />
              </Box>

              <Box>
                <Typography sx={labelStyle}>New Password</Typography>
                <TextField
                  size="small"
                  type={"password"}
                  name="newPassword"
                  fullWidth
                  value={formData.newPassword}
                  onChange={handleChange}
                  error={!!errors.newPassword}
                  helperText={errors.newPassword}
                  sx={textFieldStyles}
                />
              </Box>

              {formData.newPassword && (
                <Box sx={{ mt: -1 }}>
                  <LinearProgress
                    variant="determinate"
                    value={strength}
                    sx={{
                      height: 8,
                      borderRadius: 5,
                      bgcolor: "rgba(118,75,162,0.2)",
                      "& .MuiLinearProgress-bar": {
                        bgcolor:
                          strength < 50
                            ? "#f44336"
                            : strength < 75
                              ? "#ff9800"
                              : "#4caf50"
                      }
                    }}
                  />
                  <Typography
                    variant="caption"
                    sx={{
                      mt: 0.5,
                      display: "block",
                      color:
                        strength < 50
                          ? "#f44336"
                          : strength < 75
                            ? "#ff9800"
                            : "#4caf50"
                    }}
                  >
                    {strength < 50
                      ? "Weak"
                      : strength < 75
                        ? "Medium"
                        : "Strong"}
                  </Typography>
                </Box>
              )}

              <Button
                type="submit"
                variant="contained"
                sx={{
                  py: 1,
                  fontWeight: 600,
                  fontSize: "1rem",
                  textTransform: "none",
                  borderRadius: 3,
                  background: "linear-gradient(90deg, #667eea, #764ba2)",
                  "&:hover": {
                    background: "linear-gradient(90deg, #5a67d8, #6b46c1)"
                  }
                }}
              >
                Update Password
              </Button>
            </Box>
          </CardContent>
        </Card>
      )}
    </Box>
  );
};
