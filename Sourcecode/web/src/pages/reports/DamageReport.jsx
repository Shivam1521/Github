import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  InputLabel,
  Grid,
  TextField,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";

export const DamageReport = () => {
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [loading, setLoading] = useState(false);
  const [totalRecords, setTotalRecords] = useState(0);
  const [sortModel, setSortModel] = useState([
    {
      field: "createDate",
      sort: "desc",
    },
  ]);

  const columns = [
    {
      field: "qr",
      headerName: "QR Code Number",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "description",
      headerName: "Product Description",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "reason",
      headerName: "Reason",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "type",
      headerName: "Type",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "date",
      headerName: "Damaged On",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "name",
      headerName: "Employee Name",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "email",
      headerName: "Email",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
  ];

  const rows = [
    {
      id: 1,
      qr: "QR-0001",
      description: "Item 1 Description",
      reason: "Damaged in transit",
      type: "Master",
      date: "2025-08-05",
      name: "John Doe",
      email: "john.doe@example.com",
    },
    {
      id: 2,
      qr: "QR-0002",
      description: "Item 2 Description",
      reason: "Broken seal",
      type: "Outer",
      date: "2025-08-05",
      name: "Jane Smith",
      email: "jane.smith@example.com",
    },
    {
      id: 3,
      qr: "QR-0003",
      description: "Item 3 Description",
      reason: "Scratched surface",
      type: "Master",
      date: "2025-08-05",
      name: "Michael Johnson",
      email: "michael.johnson@example.com",
    },
    {
      id: 4,
      qr: "QR-0004",
      description: "Item 4 Description",
      reason: "Expired",
      type: "Outer",
      date: "2025-08-05",
      name: "Emily Davis",
      email: "emily.davis@example.com",
    },
    {
      id: 5,
      qr: "QR-0005",
      description: "Item 5 Description",
      reason: "Leaking package",
      type: "Master",
      date: "2025-08-05",
      name: "William Brown",
      email: "william.brown@example.com",
    },
  ];

  useEffect(() => {
    setLoading(true);
    setTimeout(() => {
      //   setRows(mockData);
      setTotalRecords(mockData.length);
      setLoading(false);
    }, 500);
  }, [page, pageSize, sortModel]);

  const handleSortModelChange = (newSortModel) => {
    setSortModel(newSortModel);
  };

  function CustomNoRowsOverlay() {
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100%",
          fontSize: "30px",
          fontWeight: "500",
        }}
      >
        <Typography
          variant="subtitle1"
          sx={{ fontWeight: "bold", fontSize: "30px" }}
        >
          No data found
        </Typography>
      </Box>
    );
  }

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
          Product QR code Damage Report
        </Typography>
      </Box>
      <hr />
      <Paper elevation={2} sx={{ mt: 3, mb: 3 }}>
        <Grid item xs={12} md="auto" sx={{ p: 2 }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 2,
              flexWrap: "wrap",
            }}
          >
            <InputLabel
              sx={{ color: "#2d3748", fontWeight: 600, minWidth: "40px" }}
            >
              From:
            </InputLabel>
            <TextField
              type="date"
              size="small"
              variant="outlined"
              InputLabelProps={{ shrink: true }}
              sx={{
                "& .MuiOutlinedInput-root": {
                  borderRadius: 2,
                  backgroundColor: "#fff",
                  "& fieldset": { borderColor: "#cbd5e0" },
                  "&:hover fieldset": { borderColor: "#0056A8" },
                  "&.Mui-focused fieldset": { borderColor: "#0056A8" },
                },
              }}
            />

            <InputLabel
              sx={{ color: "#2d3748", fontWeight: 600, minWidth: "25px" }}
            >
              To:
            </InputLabel>
            <TextField
              type="date"
              size="small"
              variant="outlined"
              InputLabelProps={{ shrink: true }}
              sx={{
                "& .MuiOutlinedInput-root": {
                  borderRadius: 2,
                  backgroundColor: "#fff",
                  "& fieldset": { borderColor: "#cbd5e0" },
                  "&:hover fieldset": { borderColor: "#0056A8" },
                  "&.Mui-focused fieldset": { borderColor: "#0056A8" },
                },
              }}
            />
          </Box>
        </Grid>

        <Box sx={{ minHeight: "auto", width: "100%" }}>
          <DataGrid
            rows={rows}
            getRowId={(row) => row.id}
            columns={columns}
            pagination
            rowCount={totalRecords}
            pageSizeOptions={[5, 10, 25, 100]}
            paginationModel={{ page, pageSize }}
            onPaginationModelChange={(model) => {
              setPage(model.page);
              setPageSize(model.pageSize);
            }}
            sortingMode="client"
            sortModel={sortModel}
            onSortModelChange={handleSortModelChange}
            // loading={loading}
            slots={{ noRowsOverlay: CustomNoRowsOverlay }}
            disableRowSelectionOnClick
            disableColumnSelector
            disableColumnMenu
            disableColumnResize
            sx={{
              ".MuiDataGrid-cell:focus, .MuiDataGrid-cell:focus-within": {
                outline: "none",
              },
              ".MuiDataGrid-columnHeader:focus, .MuiDataGrid-columnHeader:focus-within":
                {
                  outline: "none",
                },
            }}
          />
        </Box>
      </Paper>
    </React.Fragment>
  );
};
