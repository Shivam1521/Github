import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  InputLabel,
  Grid,
  TextField,
  Button,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useDispatch, useSelector } from "react-redux";
import {
  fetchScratchDetails,
  fetchScratchFile,
  reset,
  saveScratchDetails,
} from "../store/scratchGenSlice";
import { GridToolbar } from "@mui/x-data-grid/internals";
import Restricted from "../components/Restricted";
import DownloadSharpIcon from "@mui/icons-material/DownloadSharp";
import toast from "react-hot-toast";

export const ScratchGeneration = () => {
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "CreateOn",
    sortOrder: "DESC",
    userId: sessionStorage.getItem("userId"),
  });
  const [formData, setFormData] = useState({
    soNo: "",
    Qty: "",
  });
  const [errors, setErrors] = useState({});
  const dispatch = useDispatch();
  const {
    datasource,
    totalRecords,
    loading,
    error,
    fileError,
    fileLoading,
    createScratchGenError,
    createScratchGenSuccess,
  } = useSelector((state) => state.scratchGeneration);

  const columns = [
    {
      field: "soNo",
      headerName: "SO No.",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "fromNo",
      headerName: "From No.",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "toNo",
      headerName: "ToNo",
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
    {
      field: "issuedOn",
      headerName: "Issued On",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "actions",
      headerName: "Action",
      flex: 1,
      sortable: false,
      headerAlign: "center",
      align: "center",

      renderCell: (params) => (
        <Restricted path="transactions/scratch-generation" type="isEdit">
          <span style={{ color: "rgb(195, 49, 9)", cursor: "pointer" }}>
            <DownloadSharpIcon
              sx={{ mt: 1.5 }}
              // onClick={() => {
              //   dispatch(fetchScratchFile(params.row.soNo))
              //     .unwrap()
              //     .then((fileUrl) => {
              //       if (fileUrl) window.open(fileUrl, "_blank");
              //     })
              //     .catch(() => toast.error(fileError));
                 // }}
              
//              onClick={() => {
//   dispatch(fetchScratchFile(params.row.soNo))
//     .unwrap()
//     .then((files) => {

//       console.log("FILES:", files);

//       if (!Array.isArray(files) || files.length === 0) {
//         toast.error("No files found");
//         return;
//       }

//       files.forEach((fileUrl, index) => {
//         setTimeout(() => {
//           const link = document.createElement("a");
//           link.href = fileUrl;
//           link.setAttribute("download", "");
//           document.body.appendChild(link);
//           link.click();
//           document.body.removeChild(link);
//         }, index * 400);
//       });

//       toast.success("Download started");
//     })
//     .catch((err) => {
//       console.log(err);
//       toast.error("Failed to download files");
//     });
// }}


onClick={() => {
  dispatch(fetchScratchFile(params.row.soNo))
    .unwrap()
    .then((fileUrl) => {

      console.log(fileUrl);

      if (!fileUrl) {
        toast.error("File not found");
        return;
      }

      window.open(fileUrl, "_blank");
    })
    .catch((err) => {
      toast.error(err);
    });
}}
            />
          </span>
        </Restricted>
      ),
    },
  ];

  useEffect(() => {
    const storedToken = sessionStorage.getItem("token");
    if (storedToken) {
      const newRequest = {
        ...tableRequest,
        pageNo: page + 1,
        pageSize: pageSize,
      };
      dispatch(fetchScratchDetails(newRequest));
    } else {
      navigate("/login");
    }
  }, [page, pageSize, tableRequest]);

  useEffect(() => {
    if (createScratchGenSuccess) {
      toast.success(createScratchGenSuccess);
      fetchScratchDetails();
      resetForm();
      dispatch(reset());
    }
    if (createScratchGenError) {
      toast.error(createScratchGenError);
      dispatch(reset());
    }
    return () => { };
  }, [createScratchGenSuccess, createScratchGenError]);

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

  const handleSortModelChange = (newSortModel) => {
    if (newSortModel.length > 0) {
      const sortField = newSortModel[0].field;
      const sortDirection = newSortModel[0].sort;
      const updatedRequest = {
        ...tableRequest,
        sortColumn: sortField,
        sortOrder: sortDirection.toUpperCase(),
        pageNo: page + 1,
        pageSize,
      };
      setTableRequest(updatedRequest);
      dispatch(fetchScratchDetails(updatedRequest));
    }
  };
  const onFilterModelChange = (newFilterModel) => {
    if (newFilterModel.quickFilterValues?.length > 0) {
      setTableRequest((prev) => ({
        ...prev,
        searchValue: newFilterModel.quickFilterValues.join(" "),
      }));
    } else {
      setTableRequest((prev) => ({
        ...prev,
        searchValue: "",
      }));
    }
  };
  const handleChange = (field) => (event) => {
    setFormData({ ...formData, [field]: event.target.value });
    setErrors({ ...errors, [field]: "" });
  };
  const validate = () => {
    const newErrors = {};
    if (!formData.soNo) newErrors.soNo = "SO Number is required.";
    // if (!formData.Qty) newErrors.Qty = "Qty is required.";
    if (!formData.Qty) {
      newErrors.Qty = "Qty is required.";
    } else if (Number(formData.Qty) > 5000000) {
      newErrors.Qty = "Qty cannot be greater than 50,00,000.";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };
  const handleAdd = () => {
    if (validate()) {
      dispatch(saveScratchDetails(formData));
    }
  };
  const resetForm = () => {
    setFormData({
      soNo: "",
      Qty: "",
    });
    setErrors({});
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
          Scratch Generation
        </Typography>
      </Box>
      <hr />
      <Paper elevation={2} sx={{ mt: 3, mb: 3 }}>
        <Restricted path="transactions/scratch-generation" type="isAdd">
          <Grid container spacing={2} sx={{ p: 2, alignItems: "flex-start" }}>
            <Grid size={{ md: 3, xs: 3 }}>
              <InputLabel
                variant="body2"
                sx={{ mb: 0.5, fontWeight: 500, color: "#374151" }}
                error={!!errors.soNo}
              >
                SO Number
              </InputLabel>
              <TextField
                fullWidth
                size="small"
                placeholder="Enter SO Number"
                value={formData.soNo}
                onChange={handleChange("soNo")}
                error={!!errors.soNo}
                helperText={errors.soNo}
                type="number"
              />
            </Grid>

            {/* Quantity */}
            <Grid size={{ md: 3, xs: 3 }}>
              <InputLabel
                variant="body2"
                sx={{ mb: 0.5, fontWeight: 500, color: "#374151" }}
                error={!!errors.Qty}
              >
                Quantity
              </InputLabel>
              <TextField
                fullWidth
                size="small"
                placeholder="Enter Quantity"
                value={formData.Qty}
                onChange={handleChange("Qty")}
                error={!!errors.Qty}
                helperText={errors.Qty}
                type="number"
              />
            </Grid>

            {/* Add Button */}
            <Box sx={{ display: "flex", alignItems: "start", mt: 3.3 }}>
              <Button
                sx={{
                  backgroundColor: "#1E2733",
                  color: "#fff",
                  borderRadius: "8px",
                  border: "2px solid #A0AEC0",
                  fontWeight: 600,
                  textTransform: "none",
                  transition: "all 0.3s ease",
                  "&:hover": {
                    backgroundColor: "#2A3442",
                    color: "#fff",
                    borderColor: "#fff",
                  },
                }}
                onClick={handleAdd}
              >
                Add
              </Button>
              <Button
                sx={{
                  backgroundColor: "#E2E8F0",
                  color: "#4A5568",
                  borderRadius: "8px",
                  border: "2px solid #A0AEC0",
                  fontWeight: 600,
                  textTransform: "none",
                  transition: "all 0.3s ease",
                  "&:hover": {
                    backgroundColor: "#CBD5E0",
                    color: "#1A202C",
                    borderColor: "#718096",
                  },
                  ml: 1,
                }}
                onClick={resetForm}
              >
                Clear
              </Button>
            </Box>
          </Grid>
        </Restricted>

        <Box sx={{ minHeight: "auto", width: "100%" }}>
          <DataGrid
            rows={datasource}
            getRowId={(row) => row.id}
            columns={columns}
            pagination
            paginationMode="server"
            sortingMode="server"
            slotProps={{
              toolbar: {
                printOptions: { disableToolbarButton: true },
                csvOptions: { disableToolbarButton: true },
                excelOptions: { disableToolbarButton: true },
                showQuickFilter: true,
                quickFilterProps: { debounceMs: 500 },
              },
            }}
            initialState={{
              sorting: {
                sortModel: [{ field: "createDate", sort: "desc" }],
              },
            }}
            onFilterModelChange={onFilterModelChange}
            showToolbar
            onSortModelChange={handleSortModelChange}
            rowCount={totalRecords}
            pageSizeOptions={[5, 10, 25, 100]}
            paginationModel={{ page, pageSize }}
            onPaginationModelChange={(model) => {
              setPage(model.page);
              setPageSize(model.pageSize);
            }}
            loading={loading || fileLoading}
            slots={{
              toolbar: GridToolbar,
              noRowsOverlay: CustomNoRowsOverlay,
            }}
            disableRowSelectionOnClick
            disableColumnSelector
            disableColumnMenu
            disableColumnFilter
            // disableColumnResize
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
