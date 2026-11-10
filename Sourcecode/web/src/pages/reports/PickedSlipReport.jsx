import React, { useEffect, useMemo, useRef, useState } from "react";
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
  CircularProgress,
  Select,
  MenuItem,
  Checkbox,
  FormControl,
  Autocomplete,
} from "@mui/material";
import {
  DataGrid,
  GridToolbarContainer,
  GridToolbarQuickFilter,
} from "@mui/x-data-grid";
import { useDispatch, useSelector } from "react-redux";
import { fetchUnits } from "../../store/commonSlice";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import { CSVLink } from "react-csv";
import PickedSlipRptService from "../../services/PickedSlipRptService";
import { fetchPickedSlipRptData } from "../../store/pickedSlipRptSlice";
import unitAccessHook from "../../components/UnitAccessHook";

export const PickedSlipReport = () => {
  const { unitId, isSuperAdmin } = unitAccessHook();
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const today = new Date().toISOString().split("T")[0];
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "PickedOn",
    sortOrder: "DESC",
    fromDate: today,
    toDate: today,
    unitId: unitId ? String(unitId) : null,
  });

  const dispatch = useDispatch();

  const { data, totalRecords, loading, error } = useSelector(
    (state) => state.pickedSlipRpt,
  );
  const { units } = useSelector((state) => state.lists);
  useEffect(() => {
    dispatch(fetchUnits());
  }, []);

  const pickedSlipRptService = new PickedSlipRptService();
  const columns = [
    {
      field: "pickSlipNumber",
      headerName: "Pick Slip Number",
      minWidth: 150,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "sourceBU",
      headerName: "Source BU",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "destinationBU",
      headerName: "Destination BU",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "deliveryDtlId",
      headerName: "Delivery Detail Id",
      minWidth: 120,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "itemCode",
      headerName: "Item Code",
      minWidth: 120,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "quantity",
      headerName: "Quantity",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "pickedQuantity",
      headerName: "Picked Qty",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "seqNo",
      headerName: "Sequence No",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "qr",
      headerName: "QR",
      minWidth: 170,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "unitName",
      headerName: "Unit Name",
      minWidth: 220,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "itemQty",
      headerName: "Item Qty",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "qrType",
      headerName: "QR Type",
      minWidth: 100,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "printedAt",
      headerName: "Printed At",
      minWidth: 150,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "pickedOn",
      headerName: "Picked On",
      minWidth: 200,
      headerAlign: "center",
      align: "center",
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
      dispatch(fetchPickedSlipRptData(newRequest));
    } else {
      navigate("/login");
    }
  }, [page, pageSize, tableRequest]);

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

  const CustomToolbar = React.memo(() => {
    const csvLinkRef = useRef();
    const [allData, setAllData] = useState([]);

    const newRequest = { ...tableRequest, pageSize: -1 };
    useEffect(() => {
      if (allData.length > 0 && csvLinkRef.current) {
        csvLinkRef.current.link.click();
      }
    }, [allData]);

    const handleExcelExport = async () => {
      pickedSlipRptService
        .fetchPickedSlipRpt(newRequest)
        .then((response) => {
          const data = response;

          if (data && data.responseData && data.responseData.length > 0) {
            setAllData(data.responseData);
          } else {
            setAllData([]);
          }
        })
        .catch((error) => {
          setAllData([]);
        });
    };

    const headers = useMemo(
      () =>
        columns.map((col) => ({
          label: col.headerName || col.field,
          key: col.field,
        })),
      [columns],
    );

    return (
      <GridToolbarContainer>
        <GridToolbarQuickFilter debounceMs={500} />
        <Button
          sx={{
            textTransform: "none",
            borderRadius: 2,
            px: 2,
            fontWeight: 600,
            boxShadow: "0 3px 6px rgba(0,0,0,0.1)",
            "&:hover": {
              backgroundColor: "#1565c0",
              boxShadow: "0 4px 10px rgba(91, 162, 183, 0.2)",
              color: "#fff",
            },
          }}
          variant="outlined"
          startIcon={<FileDownloadIcon />}
          onClick={handleExcelExport}
        >
          Export CSV
        </Button>
        <CSVLink
          data={allData}
          headers={headers}
          filename="Printing_Report.csv"
          target="_blank"
          ref={csvLinkRef}
          style={{ display: "none" }}
        />
      </GridToolbarContainer>
    );
  });

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
          Picked Slip Report
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
              sx={{
                color: "#2d3748",
                fontWeight: 600,
                minWidth: "40px",
                fontSize: "14px",
              }}
            >
              From:
            </InputLabel>
            <TextField
              type="date"
              size="small"
              variant="outlined"
              value={tableRequest.fromDate}
              onChange={(e) => {
                setTableRequest((prev) => ({
                  ...prev,
                  fromDate: e.target.value,
                }));
              }}
              inputProps={{
                shrink: true,
                max:
                  tableRequest.toDate || new Date().toISOString().split("T")[0],
              }}
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
              sx={{
                color: "#2d3748",
                fontWeight: 600,
                minWidth: "25px",
                fontSize: "14px",
              }}
            >
              To:
            </InputLabel>
            <TextField
              type="date"
              size="small"
              variant="outlined"
              inputProps={{
                shrink: true,
                min: tableRequest.fromDate,
                max: new Date().toISOString().split("T")[0],
              }}
              value={tableRequest.toDate}
              onChange={(e) => {
                setTableRequest((prev) => ({
                  ...prev,
                  toDate: e.target.value,
                }));
              }}
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
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                gap: 2,
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
                  value={
                    tableRequest.unitId === null
                      ? [{ label: "All Units", value: "ALL" }]
                      : units?.filter((item) =>
                          tableRequest.unitId
                            ?.split(",")
                            .includes(String(item.value)),
                        ) || []
                  }
                  onChange={(_, newValue) => {
                    if (newValue.some((item) => item.value === "ALL")) {
                      setTableRequest((prev) => ({
                        ...prev,
                        unitId: null,
                      }));
                    } else {
                      setTableRequest((prev) => ({
                        ...prev,
                        unitId: newValue.map((item) => item.value).join(","),
                      }));
                    }
                  }}
                  renderOption={(props, option, { selected }) => (
                    <li {...props} key={option.value}>
                      <Checkbox sx={{ mr: 1 }} checked={selected} />
                      {option.label}
                    </li>
                  )}
                  renderInput={(params) => (
                    <TextField {...params} size="small" />
                  )}
                />
              </FormControl>
            </Box>
          </Box>
        </Grid>

        <Box sx={{ minHeight: "auto", width: "100%" }}>
          <DataGrid
            rows={data}
            columns={columns}
            getRowId={(row) => row.qr}
            pagination
            paginationMode="server"
            sortingMode="server"
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
            loading={loading}
            slotProps={{
              toolbar: {
                printOptions: { disableToolbarButton: true },
                csvOptions: { disableToolbarButton: false },
                excelOptions: { disableToolbarButton: false },
                showQuickFilter: true,
                quickFilterProps: { debounceMs: 500 },
                // onExcelExport: handleExcelExport,
              },
            }}
            slots={{
              toolbar: CustomToolbar,
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
