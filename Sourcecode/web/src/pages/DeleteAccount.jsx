import { useState, useRef } from "react";
import toast from "react-hot-toast";
import {
  Box,
  Button,
  CircularProgress,
  IconButton,
  InputAdornment,
  Paper,
  TextField,
  Typography,
  Checkbox,
  FormControlLabel,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from "@mui/material";
import PersonIcon from "@mui/icons-material/Person";
import LockIcon from "@mui/icons-material/Lock";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import logo from "../assets/images/logo.png";
import DeleteService from "../services/DeleteAccountService";
const service = new DeleteService();

const fieldSx = (hasError) => ({
  "& .MuiOutlinedInput-root": {
    fontFamily: "'Barlow', sans-serif",
    fontWeight: 500,
    fontSize: 15,
    color: "#f0f4f8",
    background: hasError ? "rgba(198,40,40,0.18)" : "rgba(255,255,255,0.10)",
    borderRadius: 1.5,
    "& fieldset": {
      borderColor: hasError ? "#ef9a9a" : "rgba(255, 255, 255)",
    },
    "&:hover fieldset": {
      borderColor: hasError ? "#ef9a9a" : "rgba(255,255,255,0.50)",
    },
    "&.Mui-focused fieldset": {
      borderColor: hasError ? "#ef9a9a" : "#ff7043",
    },
    "& input::placeholder": { color: "rgba(255,255,255,0.35)", opacity: 1 },
  },
  "& .MuiInputLabel-root": {
    fontFamily: "'Barlow', sans-serif",
    fontWeight: 500,
    fontSize: 14,
    letterSpacing: 1,
    color: "rgba(255, 255, 255)",
    "&.Mui-focused": { color: "#ff7043" },
    "&.Mui-error": { color: "#ef9a9a" },
  },
  "& .MuiFormHelperText-root": {
    fontFamily: "'Barlow', sans-serif",
    fontWeight: 600,
    fontSize: 12,
    color: "#ef9a9a",
    display: "flex",
    alignItems: "center",
    gap: 0.5,
    mt: 0.5,
    "&::before": { content: '"⚠ "', fontSize: 13 },
  },
});

// ── Component ────────────────────────────────────────────────────────────────
export const DeleteAccount = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPass, setShowPass] = useState(false);
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);
  const formRef = useRef(null);

  const validate = () => {
    const errs = {};
    if (!email.trim()) errs.email = "Email is required";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))
      errs.email = "Enter a valid email address";

    if (!password) errs.password = "Password is required";

    return errs;
  };

  const handleDeleteClick = () => {
    const errs = validate();
    setErrors(errs);

    if (Object.keys(errs).length) {
      toast.error("Please fix the errors");
      return;
    }

    setConfirmOpen(true);
  };

  const handleConfirm = async () => {
    setConfirmOpen(false);
    setLoading(true);

    try {
      const res = await service.DeleteUser({
        username: email,
        password,
      });

      if (res.responseCode == 200) {
        toast.success(res.responseMessage || "Account deleted successfully");

        setEmail("");
        setPassword("");

        setErrors({});
      } else {
        throw new Error(res?.responseMessage || "Failed to delete account");
      }
    } catch (err) {
      setErrors({ api: err.message });
      toast.error(err.message || "Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        background: "#e8eef2",
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        p: 2.5,
        fontFamily: "'Barlow', sans-serif",
      }}
    >
      <Paper
        ref={formRef}
        elevation={0}
        sx={{
          width: "100%",
          maxWidth: 460,
          borderRadius: 4,
          p: "40px 44px 44px",
          boxShadow: "0 8px 40px rgba(0,0,0,0.22)",
          background: "#44495b",
        }}
      >
        {/* ── Logo ── */}
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            gap: 1,
            pb: 2.5,
            mb: 3,
            borderBottom: "1px solid rgba(255,255,255,0.14)",
          }}
        >
          <img src={logo} alt="Logo" />
        </Box>

        {/* ── Title ── */}
        <Box
          sx={{
            textAlign: "center",
            pb: 2,
            mb: 3.5,
            borderBottom: "1px solid rgba(255,255,255,0.14)",
          }}
        >
          <Typography
            sx={{
              fontSize: 13,
              fontWeight: 700,
              letterSpacing: 2.5,
              color: "rgba(255,255,255,0.75)",
              fontFamily: "'Barlow', sans-serif",
            }}
          >
            ACCOUNT DELETION REQUEST
          </Typography>
        </Box>

        {/* ── Email Field ── */}
        <Box
          sx={{
            mb: 4,
            display: "flex",
            alignItems: "flex-start",

            gap: 1.25,
          }}
        >
          <PersonIcon
            sx={{
              mt: 1,
              color: "#fff",
              fontSize: 24,
            }}
          />
          <TextField
            fullWidth
            label="EMAIL ADDRESS"
            type="email"
            value={email}
            placeholder="Enter your email"
            onChange={(e) => {
              setEmail(e.target.value);
              if (errors.email) setErrors((p) => ({ ...p, email: undefined }));
            }}
            error={!!errors.email}
            helperText={errors.email}
            variant="outlined"
            size="small"
            InputLabelProps={{ shrink: true }}
            sx={fieldSx(!!errors.email)}
          />
        </Box>

        {/* ── Password Field ── */}
        <Box
          sx={{
            mb: 2.5,
            display: "flex",
            alignItems: "flex-start",
            gap: 1.25,
          }}
        >
          <LockIcon
            sx={{
              mt: 1,
              color: "#fff",
              fontSize: 24,
            }}
          />
          <TextField
            fullWidth
            label="PASSWORD"
            type={showPass ? "text" : "password"}
            value={password}
            placeholder="Enter your password"
            onChange={(e) => {
              setPassword(e.target.value);
              if (errors.password)
                setErrors((p) => ({ ...p, password: undefined }));
            }}
            error={!!errors.password}
            helperText={errors.password}
            variant="outlined"
            size="small"
            InputLabelProps={{ shrink: true }}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPass((p) => !p)}
                    edge="end"
                    size="small"
                    sx={{ color: "rgba(255,255,255,0.45)" }}
                  >
                    {showPass ? (
                      <VisibilityOff fontSize="small" />
                    ) : (
                      <Visibility fontSize="small" />
                    )}
                  </IconButton>
                </InputAdornment>
              ),
            }}
            sx={fieldSx(!!errors.password)}
          />
        </Box>

        {/* ── Submit Button ── */}
        <Button
          fullWidth
          variant="contained"
          onClick={handleDeleteClick}
          disabled={loading}
          startIcon={
            loading ? (
              <CircularProgress
                size={18}
                sx={{ color: "rgba(255,255,255,0.7)" }}
              />
            ) : (
              <DeleteForeverIcon />
            )
          }
          sx={{
            py: 1.6,
            background: "linear-gradient(135deg, #d32f2f, #b71c1c)",
            fontFamily: "'Barlow', sans-serif",
            fontWeight: 800,
            fontSize: 15,
            letterSpacing: 1.5,
            borderRadius: 2,
            boxShadow: "0 4px 16px rgba(183,28,28,0.45)",
            textTransform: "none",
            color: "#fff",
            "&:hover": {
              background: "linear-gradient(135deg, #c62828, #a31515)",
              boxShadow: "0 6px 20px rgba(183,28,28,0.55)",
              transform: "translateY(-1px)",
            },
            "&:disabled": {
              background: "rgba(183,28,28,0.35)",
              color: "rgba(255,255,255,0.45)",
              boxShadow: "none",
            },
            transition: "all 0.2s",
          }}
        >
          {loading ? "Processing..." : "DELETE ACCOUNT"}
        </Button>
      </Paper>

      {/* ── Confirmation Dialog ── */}
      <Dialog
        open={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        PaperProps={{
          sx: {
            background: "#ffffff",
            borderRadius: 3,
            boxShadow: "0 8px 40px rgba(0,0,0,0.2)",
            p: 1,
            maxWidth: 400,
            width: "100%",
          },
        }}
      >
        <DialogTitle
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1.5,
            fontFamily: "'Barlow', sans-serif",
            fontWeight: 700,
            fontSize: 16,
            letterSpacing: 1.5,
            color: "#b71c1c",
            pb: 1,
          }}
        >
          <WarningAmberIcon sx={{ color: "#d32f2f", fontSize: 22 }} />
          CONFIRM DELETION
        </DialogTitle>

        <DialogContent
          sx={{
            borderTop: "1px solid rgba(0,0,0,0.10)",
            pt: "16px !important",
          }}
        >
          <Typography
            sx={{
              fontFamily: "'Barlow', sans-serif",
              fontSize: 14,
              color: "rgba(0, 0, 0, 0.85)",
              lineHeight: 1.6,
            }}
          >
            Are you sure you want to permanently delete this account?
          </Typography>
        </DialogContent>

        <DialogActions sx={{ pt: 2, gap: 1, pb: 1, px: 2 }}>
          <Button
            onClick={() => setConfirmOpen(false)}
            sx={{
              fontFamily: "'Barlow', sans-serif",
              fontWeight: 600,
              fontSize: 13,
              color: "rgba(0,0,0,0.5)",
              textTransform: "none",
              borderRadius: 1.5,
              px: 2.5,
              "&:hover": {
                background: "rgba(0,0,0,0.06)",
                color: "rgba(0,0,0,0.8)",
              },
            }}
          >
            Cancel
          </Button>
          <Button
            onClick={handleConfirm}
            variant="contained"
            startIcon={<DeleteForeverIcon fontSize="small" />}
            sx={{
              fontFamily: "'Barlow', sans-serif",
              fontWeight: 700,
              fontSize: 13,
              textTransform: "none",
              borderRadius: 1.5,
              px: 2.5,
              background: "linear-gradient(135deg, #d32f2f, #b71c1c)",
              color: "#fff",
              boxShadow: "0 4px 12px rgba(183,28,28,0.45)",
              "&:hover": {
                background: "linear-gradient(135deg, #c62828, #a31515)",
                boxShadow: "0 6px 16px rgba(183,28,28,0.55)",
              },
            }}
          >
            Yes, Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};
