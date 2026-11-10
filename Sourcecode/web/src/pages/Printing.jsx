import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  Grid,
  Select,
  MenuItem,
  InputLabel,
  FormControl,
  Button,
  Stack,
  CircularProgress,
} from "@mui/material";
import CloudUploadIcon from "@mui/icons-material/CloudUpload";
import DownloadIcon from "@mui/icons-material/Download";
import { useDispatch, useSelector } from "react-redux";
import { reset, serialUpload } from "../store/printingSlice";
import toast from "react-hot-toast";
import { fetchList } from "../store/commonSlice";

export const Printing = () => {
  const [type, setType] = useState("");
  const [file, setFile] = useState(null);
  const dispatch = useDispatch();
  const { loading, uploadError, uploadSuccess } = useSelector(
    (state) => state.serialUpload
  );
  const { ["Print Source"]: PrintSource } = useSelector(
    (state) => state.lists.listData
  );
  useEffect(() => {
    dispatch(fetchList("Print Source"));
  }, []);

  useEffect(() => {
    if (uploadSuccess) {
      toast.success(uploadSuccess);
      setFile(null);
      dispatch(reset());
    }
    if (uploadError) {
      toast.error(uploadError);
      dispatch(reset());
    }
    return () => {};
  }, [uploadSuccess, uploadError]);

 const handleFileChange = (e) => {
  const selected = e.target.files[0];

  if (!selected) return;

  const allowedExtensions = [".xlsx", ".xls", ".csv"];
  const fileName = selected.name.toLowerCase();

  const isValid = allowedExtensions.some((ext) =>
    fileName.endsWith(ext)
  );

  if (!isValid) {
    toast.error("Only Excel or CSV files (.xlsx, .xls, .csv) are allowed.");
    setFile(null);
    e.target.value = "";
    return;
  }

  setFile(selected);
};

  const handleSubmit = () => {
    if (!type) {
      alert("Please select upload type.");
      return;
    }
    if (!file) {
      alert("Please upload a file.");
      return;
    }
    const formData = new FormData();
    formData.append("type", type);
    formData.append("uploadedFile", file);

    dispatch(serialUpload(formData));
  };

  return (
    <React.Fragment>
      <Box sx={{ display: "flex", alignItems: "center", gap: 1.5 }}>
        <Typography
          variant="h5"
          sx={{
            fontWeight: 600,
            color: "#333",
            letterSpacing: "0.5px",
          }}
        >
          Printing
        </Typography>
      </Box>
      <hr />
      <Paper
        elevation={3}
        sx={{
          p: 4,
          backgroundColor: "#fff",
          borderRadius: 2,
          mt: 3,
        }}
      >
        <Grid container spacing={3}>
          {/* Upload Type Dropdown */}
          <Grid size={{ md: 3, xs: 6 }}>
            <FormControl fullWidth size="small">
              <InputLabel sx={{ color: "#000" }}>Upload Type</InputLabel>
              <Select
                value={type}
                onChange={(e) => {
                  setType(e.target.value);
                  setFile(null);
                }}
                label="Upload Type"
              >
                {PrintSource?.map((option) => (
                  <MenuItem key={option.value} value={option.label}>
                    {option.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>

        {/* Upload Card */}
        {type && (
          <Box
            sx={{ mt: 4 }}
            onDragOver={(e) => e.preventDefault()}
            onDrop={(e) => {
              e.preventDefault();
            
              const droppedFile = e.dataTransfer.files[0];
            
              if (!droppedFile) return;
            
              const allowedExtensions = [".xlsx", ".xls", ".csv"];
              const fileName = droppedFile.name.toLowerCase();
            
              const isValid = allowedExtensions.some((ext) =>
                fileName.endsWith(ext)
              );
            
              if (!isValid) {
                toast.error("Only Excel or CSV files (.xlsx, .xls, .csv) are allowed.");
                setFile(null);
                return;
              }
            
              setFile(droppedFile);
            }}
                      >
                        <Paper
              elevation={3}
              sx={{
                p: 3,
                borderRadius: 2,
                backgroundColor: "#f9fafb",
                border: "2px dashed #cbd5e0",
                textAlign: "center",
                color: "#2d3748",
                transition: "all 0.3s ease",
                "&:hover": {
                  borderColor: "#0056A8",
                  backgroundColor: "#f1f5f9",
                },
              }}
            >
              <Stack spacing={2} alignItems="center">
                <CloudUploadIcon sx={{ fontSize: 50, color: "#64b5f6" }} />

                <Typography variant="h6">Upload File</Typography>

                <Typography variant="body2" sx={{ color: "#bbb" }}>
                  Drag and drop your file here or click below to upload
                </Typography>

                <Button
                  variant="outlined"
                  component="label"
                  sx={{
                    color: "#64b5f6",
                    borderColor: "#64b5f6",
                    "&:hover": {
                      backgroundColor: "#64b5f620",
                      borderColor: "#64b5f6",
                    },
                  }}
                >
                  Choose File
                  <input
                    type="file"
                    hidden
                    onChange={handleFileChange}
                    accept=".xlsx,.xls,.csv"
                  />
                </Button>

                {file && (
                  <Typography variant="body2" sx={{ color: "#333" }}>
                    Selected: <strong>{file.name}</strong>
                  </Typography>
                )}

                <Button
                  startIcon={<DownloadIcon />}
                  href="/templates/template.xlsx"
                  download
                  sx={{
                    mt: 2,
                    color: "#64b5f6",
                    textTransform: "none",
                    fontWeight: 500,
                    fontSize: "0.95rem",

                    justifyContent: "flex-start",
                    "&:hover": {
                      color: "#90caf9",
                      textDecoration: "underline",
                      backgroundColor: "transparent",
                    },
                  }}
                >
                  Download Sample Template
                </Button>
              </Stack>
            </Paper>
            <Grid justifyContent="center" display="flex">
              <Button
                variant="contained"
                onClick={handleSubmit}
                disabled={loading}
                sx={{
                  mt: 3,
                  background: "#0658a0",
                  fontWeight: 600,
                  textTransform: "none",
                  borderRadius: "8px",
                  "&:hover": {
                    background: "#003a70",
                  },
                }}
              >
                {loading ? <CircularProgress size={20} /> : "Submit"}
              </Button>
            </Grid>
          </Box>
        )}
      </Paper>
    </React.Fragment>
  );
};
