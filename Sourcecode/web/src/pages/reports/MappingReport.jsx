import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  InputLabel,
  Grid,
  TextField,
  Link,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";

export const MappingReport = () => {
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
  const [selectedId, setSelectedId] = useState(null);

  const handleQrClick = (row) => {
    setSelectedId(row);
    setQrDialogOpen(true);
  };

  const columns = [
    {
      field: "id",
      headerName: "ID",
      flex: 1,
      headerAlign: "center",
      align: "center",
      renderCell: (params) => (
        <Link
          variant="text"
          onClick={() => handleQrClick(params.row)}
          sx={{ color: "#0056A8", fontWeight: 600, textTransform: "none" }}
        >
          {params.row.id}
        </Link>
      ),
    },
    {
      field: "date",
      headerName: "Date",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "itemName",
      headerName: "Item Name",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "qty",
      headerName: "Quantity",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
  ];

  const rows = [
    {
      id: "MASTER-1",
      date: "2025-08-04 10:00:00",
      itemName: "Item 1 Description",
      qty: 100,
      sIds: ["OUTER-1", "OUTER-2", "OUTER-3"],
    },
    {
      id: "MASTER-2",
      date: "2025-08-04 20:00:00",
      itemName: "Item 2 Description",
      qty: 200,
      sIds: ["OUTER-4", "OUTER-5"],
    },
    {
      id: "MASTER-3",
      date: "2025-08-04 16:00:00",
      itemName: "Item 3 Description",
      qty: 150,
      sIds: ["OUTER-6"],
    },
    {
      id: "MASTER-4",
      date: "2025-08-04 18:00:00",
      itemName: "Item 4 Description",
      qty: 80,
      sIds: ["OUTER-7", "OUTER-8"],
    },
    {
      id: "MASTER-5",
      date: "2025-08-04 10:00:00",
      itemName: "Item 5 Description",
      qty: 300,
      sIds: [
        "OUTER-9",
        "OUTER-10",
        "OUTER-11",
        "OUTER-12",
        "OUTER-13",
        "OUTER-14",
      ],
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
          Mapping Report
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
        <DialogTitle textAlign="center">Secondary Mapping</DialogTitle>
        <DialogContent>
          {selectedId && (
            <Box
              sx={{
                backgroundColor: "#ffffff",
                borderRadius: 2,
                p: 3,
                color: "#1a202c",
                boxShadow: "0px 2px 10px rgba(0,0,0,0.08)",
                maxWidth: 500,
                border: "1px solid #e2e8f0",
              }}
            >
              <Grid container spacing={2}>
                <Grid size={{ md: 12 }}>
                  {/* <Typography variant="caption" sx={{ opacity: 0.7 }}>
                    Secondary Mapping
                  </Typography> */}

                  <Box
                    sx={{
                      display: "flex",
                      flexWrap: "wrap",
                      justifyContent: "center",
                      gap: 1,
                      //   mt: 1,
                    }}
                  >
                    {selectedId.sIds?.map((qr, index) => (
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
              backgroundColor: "#B71C1C",
              fontWeight: 600,
              textTransform: "none",
              borderRadius: "8px",
              px: 3,
              py: 0.5,
              mr: 2,
              "&:hover": {
                backgroundColor: "#C62828",
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
