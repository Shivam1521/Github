import React, { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  InputLabel,
  Grid,
  TextField,
  Button,
  Autocomplete,
  FormControl,
  Checkbox,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useDispatch, useSelector } from "react-redux";
import { GridToolbar } from "@mui/x-data-grid/internals";
import { fetchPickSlipData } from "../../store/pickSlipStatusSlice";
import unitAccessHook from "../../components/UnitAccessHook";
import { fetchUnits } from "../../store/commonSlice";

export const PickSlipReceiptStatus = () => {
  const [formData, setFormData] = useState({
    pickslipNumber: "",
  });
  const [errors, setErrors] = useState({});
  const [selectedUnits, setSelectedUnits] = useState([]);
  const [selectedUnit, setSelectedUnit] = useState(null);
  const dispatch = useDispatch();
  const { data, loading, error } = useSelector((state) => state.pickSlipStatus);
  const { unitId, isSuperAdmin } = unitAccessHook();

  const { units } = useSelector((state) => state.lists);
  useEffect(() => {
    dispatch(fetchUnits());
    dispatch(fetchPickSlipData(""));
  }, []);
  const columns = [
    {
      field: "pickSlipNumber",
      headerName: "Pick Slip Number",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "deliveryDetailId",
      headerName: "Delivery Detail Id",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "itemCode",
      headerName: "Item Code",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "seqNo",
      headerName: "Sequence No",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "qrType",
      headerName: "QR Type",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "location",
      headerName: "Location",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "qr",
      headerName: "QR",
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
  ];
  useEffect(() => {
    if (unitId && units?.length) {
      const defaultUnit = units.find((u) => String(u.value) === String(unitId));

      if (defaultUnit) {
        setSelectedUnits([defaultUnit]);
        setSelectedUnit(String(defaultUnit.value));
      }
    }
  }, [unitId, units]);
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

  const handleChange = (field) => (event) => {
    setFormData({ ...formData, [field]: event.target.value });
    setErrors({ ...errors, [field]: "" });
  };

  const validate = () => {
    const newErrors = {};
    if (!formData.pickslipNumber)
      newErrors.pickslipNumber = "Pickslip Number is required.";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleFetch = () => {
    if (validate()) {
      dispatch(fetchPickSlipData(formData.pickslipNumber));
    }
  };

  const resetForm = () => {
    setFormData({
      pickslipNumber: "",
    });
    setErrors({});
  };
  const filteredData = React.useMemo(() => {
    if (!data) return [];
    if (!selectedUnit) return data;

    const unitIds = selectedUnit.split(",");

    return data;
  }, [data, selectedUnit]);
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
          Pick Slip Receipt Status
        </Typography>
      </Box>
      <hr />
      <Paper elevation={2} sx={{ mt: 3, mb: 3 }}>
        <Grid container spacing={2} sx={{ p: 2, alignItems: "flex-start" }}>
          <Grid size={{ md: 3, xs: 3 }}>
            <InputLabel
              variant="body2"
              sx={{ mb: 0.5, fontWeight: 500, color: "#374151" }}
              error={!!errors.pickslipNumber}
            >
              Pick Slip Number
            </InputLabel>
            <TextField
              fullWidth
              size="small"
              placeholder="Enter Pick Slip Number"
              value={formData.pickslipNumber}
              onChange={handleChange("pickslipNumber")}
              error={!!errors.pickslipNumber}
              helperText={errors.pickslipNumber}
              type="number"
            />
          </Grid>

          {/* Fetch Button */}
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
              onClick={handleFetch}
            >
              Fetch
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
          {/* <Box
            sx={{
              marginLeft: "auto",
            }}
          >
            <InputLabel
              sx={{
                color: "#2d3748",
                fontWeight: 600,
                fontSize: "14px",
                minWidth: "80px",
              }}
            >
              Select Unit:
            </InputLabel>
            <FormControl
              fullWidth
              size="small"
              sx={{ maxWidth: "fit-content", minWidth: 200 }}
            >
              <Autocomplete
                multiple
                disableCloseOnSelect
                options={[
                  { label: "All Units", value: "ALL" },
                  ...(units || []),
                ]}
                disabled={!isSuperAdmin}
                getOptionLabel={(option) => option.label || ""}
                isOptionEqualToValue={(option, value) =>
                  option.value === value.value
                }
                value={selectedUnits}
                onChange={(_, newValue) => {
                  const isAllSelected = newValue.some(
                    (item) => item.value === "ALL",
                  );

                  if (isAllSelected) {
                    setSelectedUnits([{ label: "All Units", value: "ALL" }]);
                    setSelectedUnit(null);
                    return;
                  }

                  const filtered = newValue.filter(
                    (item) => item.value !== "ALL",
                  );

                  setSelectedUnits(filtered);
                  setSelectedUnit(
                    filtered.length
                      ? filtered.map((item) => item.value).join(",")
                      : null,
                  );
                }}
                renderOption={(props, option, { selected }) => (
                  <li {...props} key={option.value}>
                    <Checkbox sx={{ mr: 1 }} checked={selected} />
                    {option.label}
                  </li>
                )}
                renderInput={(params) => <TextField {...params} size="small" />}
              />
            </FormControl>
          </Box> */}
        </Grid>

        <Box sx={{ minHeight: "auto", width: "100%" }}>
          <DataGrid
            rows={filteredData || []}
            getRowId={(row) => row.qr}
            columns={columns}
            pagination
            pageSizeOptions={[5, 10, 25, 100]}
            loading={loading}
            showToolbar
            slots={{
              toolbar: GridToolbar,
              noRowsOverlay: CustomNoRowsOverlay,
            }}
            slotProps={{
              toolbar: {
                printOptions: { disableToolbarButton: true },
                csvOptions: {
                  disableToolbarButton: data?.length > 0 ? false : true,
                  fileName: "PickSlip_Receipt_Status",
                },

                showQuickFilter: true,
                quickFilterProps: { debounceMs: 500 },
              },
            }}
            initialState={{
              pagination: {
                paginationModel: {
                  pageSize: 5,
                  page: 0,
                },
              },
              sorting: {
                sortModel: [{ field: "createDate", sort: "desc" }],
              },
            }}
            disableRowSelectionOnClick
            disableColumnSelector
            disableColumnMenu
            disableColumnFilter
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
