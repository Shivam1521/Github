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

export const DataReport = () => {
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
      field: "source",
      headerName: "Source Destination",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "status",
      headerName: "Status",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "recordCounts",
      headerName: "Record Counts",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
  ];
  const rows = [
    {
      id: 1,
      source: "ERP",
      status: "Success",
      recordCounts: 100,
    },
    {
      id: 2,
      source: "WMS",
      status: "Failure",
      recordCounts: 200,
    },
    {
      id: 3,
      source: "Loyalty",
      status: "Success",
      recordCounts: 150,
    },
    {
      id: 4,
      source: "Warranty",
      status: "Failure",
      recordCounts: 80,
    },
  ];

  useEffect(() => {
    setLoading(true);
    setTimeout(() => {
      //   setRows(mockData);
      //   setTotalRecords(mockData.length);
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
          Data Pushed / Pulled Report
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
