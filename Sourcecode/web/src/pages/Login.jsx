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
  useTheme,
  CircularProgress,
} from "@mui/material";
import { Visibility, VisibilityOff, Lock, Person } from "@mui/icons-material";
import { motion } from "framer-motion";
import bgImage from "../assets/images/bgImage.png";
import { useNavigate } from "react-router-dom";
import logo from "../assets/images/logo.png";
import LoginService from "../services/LoginService";
import toast from "react-hot-toast";
import { usePermissions } from "../helper/permissionContext";
import ReCAPTCHA from "react-google-recaptcha";
import { setUnit } from "../store/unitSlice";
import { useDispatch } from "react-redux";

export default function LoginScreen({ setIsLoggedIn }) {
  const [captchaToken, setCaptchaToken] = useState(null);
  const [form, setForm] = useState({ username: "", password: "" });
  const [errors, setErrors] = useState({});
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const { refreshPermissions } = usePermissions();
  const theme = useTheme();
  const loginService = new LoginService();
  const navigate = useNavigate("");
  const [failedAttempts, setFailedAttempts] = useState(0);
  const dispatch = useDispatch();

  if (failedAttempts >= 3) showCaptcha = true;

  const handleCaptchaChange = (token) => {
    setCaptchaToken(token);
  };

  const handleCaptchaExpired = () => {
    setCaptchaToken(null);
  };

  const handleChange = (field) => (e) => {
    setForm({ ...form, [field]: e.target.value });
    setErrors({ ...errors, [field]: "" });
  };

  const togglePasswordVisibility = () => setShowPassword((prev) => !prev);

  const validate = () => {
    const newErrors = {};

    if (!form.username.trim()) newErrors.username = "Email is required";
    if (!form.password.trim()) newErrors.password = "Password is required";
    if (!captchaToken) newErrors.captcha = "Please verify captcha";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validate()) return;

    setLoading(true);

    try {
      const response = await loginService.LoginUser({
        ...form,
        captchaToken,
      });

      if (response.responseCode === "200") {
        const user = response.responseData;

        sessionStorage.setItem("token", user.token);
        sessionStorage.setItem("refreshToken", user.refreshToken);
        sessionStorage.setItem("userId", user.userId);
        sessionStorage.setItem("email", user.email);
        sessionStorage.setItem("userType", user.userType);
        sessionStorage.setItem("roleId", user.roleId);
        sessionStorage.setItem("unitId", user.unitId);
        sessionStorage.setItem("userName", user.userName);
        sessionStorage.setItem("isSA", user.isSA);
        dispatch(setUnit(user.unitId));
        await refreshPermissions();
        setIsLoggedIn(true);
        toast.success("Login Successful");
        navigate("/dashboard");
      } else {
        toast.error(response.responseMessage || "Login failed.");
        captchaRef.current.reset();
        setCaptchaToken(null);
      }
    } catch (error) {
      console.error("Login error:", error);
      toast.error("Login failed.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        position: "relative",
        backgroundImage: `url(${bgImage})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "flex-start",
        backdropFilter: "blur(4px)",
        px: 3,
      }}
    >
      <Box sx={{ position: "absolute", top: 20, left: 15 }}>
        <img src={logo} alt="" />
      </Box>
      <motion.div
        initial={{ scale: 0.9, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        transition={{ duration: 0.5 }}
      >
        <Card
          sx={{
            maxWidth: 400,
            p: 3,
            borderRadius: 4,
            boxShadow: "0 8px 24px rgba(0,0,0,0.4)",
            background: "rgba(255,255,255,0.1)",
            backdropFilter: "blur(10px)",
            color: "#fff",
            ml: { xs: 0, sm: 5 },
          }}
        >
          <CardContent>
            <Typography
              variant="h5"
              fontWeight="bold"
              textAlign="center"
              gutterBottom
            >
              LOGIN
            </Typography>
            <Typography variant="body2" mb={3} textAlign="center">
              Please enter your credentials
            </Typography>
            <Box component="form" onSubmit={handleSubmit}>
              <input
                type="text"
                name="fake-username"
                autoComplete="username"
                style={{ display: "none" }}
              />
              <TextField
                label="Email"
                value={form.username}
                onChange={handleChange("username")}
                fullWidth
                type="email"
                variant="outlined"
                margin="normal"
                error={!!errors.username}
                helperText={errors.username}
                name="username"
                autoComplete="new-username"
                inputProps={{
                  autoComplete: "new-username",

                  form: {
                    autoComplete: "off",
                  },
                }}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Person sx={{ color: "#fff" }} />
                    </InputAdornment>
                  ),
                  style: { color: "#fff" },
                }}
                InputLabelProps={{ style: { color: "#fff" } }}
                sx={{
                  "& .MuiOutlinedInput-root": {
                    "& fieldset": { borderColor: "#fff" },
                    "&:hover fieldset": { borderColor: "#fff" },
                    "&.Mui-focused fieldset": { borderColor: "#fff" },
                  },
                  "& input:-webkit-autofill": {
                    WebkitBoxShadow: " rgba(255,255,255,0.1)",
                    WebkitTextFillColor: "#fff",
                    transition: "background-color 5000s ease-in-out 0s",
                  },
                }}
              />

              <TextField
                label="Password"
                type={showPassword ? "text" : "password"}
                value={form.password}
                onChange={handleChange("password")}
                fullWidth
                variant="outlined"
                margin="normal"
                autoComplete="off"
                error={!!errors.password}
                helperText={errors.password}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Lock sx={{ color: "#fff" }} />
                    </InputAdornment>
                  ),
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton onClick={togglePasswordVisibility} edge="end">
                        {showPassword ? (
                          <VisibilityOff sx={{ color: "#fff" }} />
                        ) : (
                          <Visibility sx={{ color: "#fff" }} />
                        )}
                      </IconButton>
                    </InputAdornment>
                  ),
                  style: { color: "#fff" },
                }}
                InputLabelProps={{ style: { color: "#fff" } }}
                sx={{
                  "& .MuiOutlinedInput-root": {
                    "& fieldset": { borderColor: "#fff" },
                    "&:hover fieldset": { borderColor: "#fff" },
                    "&.Mui-focused fieldset": { borderColor: "#fff" },
                  },
                  "& input:-webkit-autofill": {
                    WebkitBoxShadow: " rgba(255,255,255,0.1)",
                    WebkitTextFillColor: "#fff",
                    transition: "background-color 5000s ease-in-out 0s",
                  },
                }}
              />

              <Box mt={2} display="flex" justifyContent="center">
                <ReCAPTCHA
                  sitekey={import.meta.env.VITE_RECAPTCHA_SITE_KEY}
                  onChange={handleCaptchaChange}
                  onExpired={handleCaptchaExpired}
                />
              </Box>

              {errors.captcha && (
                <Typography
                  variant="caption"
                  color="error"
                  display="block"
                  textAlign="center"
                  mt={1}
                >
                  {errors.captcha}
                </Typography>
              )}

              <Button
                variant="contained"
                color="primary"
                type="submit"
                fullWidth
                disabled={loading}
                sx={{
                  mt: 3,
                  py: 1.5,
                  fontWeight: "bold",
                  borderRadius: 2,
                  background: `linear-gradient(45deg, ${theme.palette.primary.main}, ${theme.palette.secondary.main})`,
                  "&:disabled": {
                    background: `linear-gradient(45deg, ${theme.palette.primary.main}, ${theme.palette.secondary.main})`,
                    opacity: 0.8,
                  },
                }}
              >
                {loading ? (
                  <CircularProgress size={24} sx={{ color: "#fff" }} />
                ) : (
                  "Login"
                )}
              </Button>
            </Box>
          </CardContent>
        </Card>
      </motion.div>
    </Box>
  );
}
