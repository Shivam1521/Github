import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  InputLabel,
  Grid,
  TextField,
  Button,
  Link,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Divider,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";

export const MoveOrderReport = () => {
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

  const [qrDialogOpen, setQrDialogOpen] = useState(false);
  const [selectedQrData, setSelectedQrData] = useState(null);

  const handleQrClick = (row) => {
    setSelectedQrData(row);
    setQrDialogOpen(true);
  };

  const columns = [
    {
      field: "moveOrder",
      headerName: "Move Order",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "pallet",
      headerName: "Pallet",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "masterQr",
      headerName: "Master QR",
      flex: 1.5,
      headerAlign: "center",
      align: "center",
      renderCell: (params) => (
        <Link
          variant="text"
          onClick={() => handleQrClick(params.row)}
          sx={{ color: "#0056A8", fontWeight: 600, textTransform: "none" }}
        >
          {params.row.masterQr}
        </Link>
      ),
    },
    {
      field: "status",
      headerName: "QC S tatus",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
  ];

  const rows = [
    {
      id: 1,
      moveOrder: "MO-001",
      pallet: "P-101",
      masterQr: "QR-1001",
      itemCode: "ITEM-5001",
      outerQrs: ["OUTER-2001", "OUTER-2002"],
      status: "Approved",
    },
    {
      id: 2,
      moveOrder: "MO-002",
      pallet: "P-102",
      masterQr: "QR-1002",
      itemCode: "ITEM-6001",
      outerQrs: ["OUTER-3001", "OUTER-3002", "OUTER-3003", "OUTER-3004"],
      status: "Rejected",
    },
    {
      id: 3,
      moveOrder: "MO-003",
      pallet: "P-103",
      masterQr: "QR-1003",
      itemCode: "ITEM-7001",
      outerQrs: ["OUTER-4001", "OUTER-4002", "OUTER-4003", "OUTER-4004"],
      status: "Approved",
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
          Move Order Report
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
      <Dialog open={qrDialogOpen} onClose={() => setQrDialogOpen(false)}>
        <DialogTitle
          textAlign="center"
          sx={{
            textAlign: "center",
            fontWeight: 600,
            color: "#1a202c",
            borderBottom: "1px solid #e2e8f0",
            pb: 1.5,
          }}
        >
          QR Details
        </DialogTitle>
        <DialogContent>
          {selectedQrData && (
            <Box
              sx={{
                backgroundColor: "#ffffff",
                borderRadius: 2,
                p: 3,
                color: "#1a202c",
                boxShadow: "0px 2px 10px rgba(0,0,0,0.08)",
                maxWidth: 500,
                border: "1px solid #e2e8f0",
                mt: 2,
              }}
            >
              <Grid container spacing={2}>
                <Grid size={{ md: 6 }}>
                  <Typography variant="caption" sx={{ opacity: 0.7 }}>
                    Item Code
                  </Typography>
                  <Typography variant="body1" sx={{ fontWeight: 500 }}>
                    {selectedQrData.itemCode}
                  </Typography>
                </Grid>

                <Grid size={{ md: 12 }}>
                  <Typography variant="caption" sx={{ opacity: 0.7 }}>
                    Outer QR Codes
                  </Typography>

                  <Box
                    sx={{
                      display: "flex",
                      flexWrap: "wrap",
                      gap: 1,
                      mt: 1,
                    }}
                  >
                    {selectedQrData.outerQrs?.map((qr, index) => (
                      <Box
                        key={index}
                        sx={{
                          background: "#edf2f7",
                          px: 1.5,
                          py: 0.5,
                          borderRadius: "6px",
                          fontSize: "0.8rem",
                          fontWeight: 500,
                          color: "#2d3748",
                        }}
                      >
                        {qr}
                      </Box>
                    ))}
                  </Box>
                </Grid>
              </Grid>
            </Box>
          )}
        </DialogContent>

        <DialogActions>
          <Button
            onClick={() => setQrDialogOpen(false)}
            variant="contained"
            sx={{
              backgroundColor: "#B71C1C", // Dark red
              color: "#fff",
              fontWeight: 600,
              textTransform: "none",
              borderRadius: "8px",
              px: 3,
              py: 0.5,
              mr: 2,
              "&:hover": {
                backgroundColor: "#C62828", // Slightly lighter on hover
              },
            }}
          >
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </React.Fragment>
  );
};
