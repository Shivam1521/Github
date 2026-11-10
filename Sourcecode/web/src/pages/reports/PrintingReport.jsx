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
import {
  fetchJobData,
  fetchPrintingData,
} from "../../store/printingReportSlice";
import { fetchList, fetchUnits } from "../../store/commonSlice";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import { CSVLink } from "react-csv";
import PrintingRptService from "../../services/PrintingRptService";
import unitAccessHook from "../../components/UnitAccessHook";

export const PrintingReport = () => {
  const { unitId, isSuperAdmin } = unitAccessHook();
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "CreateDate",
    sortOrder: "DESC",
    fromDate: "",
    toDate: "",
    type: "",
    unitId: unitId ? String(unitId) : null,
  });

  const dispatch = useDispatch();
  const { ["Print Source"]: PrintSource } = useSelector(
    (state) => state.lists.listData,
  );
  const { units } = useSelector((state) => state.lists);
  const { data, totalRecords, loading, error, jobData, jobLoading } =
    useSelector((state) => state.printingRpt);

  useEffect(() => {
    dispatch(fetchUnits());
  }, []);

  const printingRptService = new PrintingRptService();
  const columns = [
    {
      field: "srNo",
      headerName: "Serial No.",
      flex: 1,
      headerAlign: "center",
      align: "center",
      renderCell: (params) => (
        <Link
          variant="text"
          onClick={() => {
            const id = params.row.srNo;
            setTimeout(() => dispatch(fetchJobData(id)), 0);
            setDialogOpen(true);
          }}
          sx={{ color: "#0056A8", fontWeight: 600, textTransform: "none" }}
        >
          {params.row.srNo}
        </Link>
      ),
    },
    {
      field: "itemCode",
      headerName: "Item Code",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "itemDescription",
      headerName: "Description",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "qtyToBeDispatched",
      headerName: "Qty to be Dispatched",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "type",
      headerName: "Source Type",
      flex: 1,
      filterable: true,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "prodLocation",
      headerName: "Location",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "createdOn",
      headerName: "Created On",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "fromNoOuter",
      headerName: "From No. Outer",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "toNoOuter",
      headerName: "To No. Outer",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "fromNoMaster",
      headerName: "From No. Master",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "toNoMaster",
      headerName: "To No. Master",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
  ];

  useEffect(() => {
    dispatch(fetchList("Print Source"));
  }, []);

  useEffect(() => {
    const storedToken = sessionStorage.getItem("token");
    if (storedToken) {
      const newRequest = {
        ...tableRequest,
        pageNo: page + 1,
        pageSize: pageSize,
      };
      dispatch(fetchPrintingData(newRequest));
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
      dispatch(fetchPrintingData(updatedRequest));
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
      printingRptService
        .fetchPrintingData(newRequest)
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
      <GridToolbarContainer
        sx={{ display: "flex", gap: 2, alignItems: "center" }}
      >
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
          Printing Report
        </Typography>
      </Box>
      <hr />
      <Paper elevation={2} sx={{ mt: 3, mb: 3 }}>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 2,
            p: 2,
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
              marginLeft: "auto",
              display: "flex",
              gap: 2,
              alignItems: "center",
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
              Select Source Type:
            </InputLabel>
            <FormControl size="small" sx={{ minWidth: 200 }}>
              <Select
                value={tableRequest.type || ""}
                onChange={(e) => {
                  setTableRequest((prev) => ({
                    ...prev,
                    type: e.target.value,
                  }));
                }}
                displayEmpty
              >
                <MenuItem value="">All</MenuItem>
                {PrintSource?.map((option) => (
                  <MenuItem key={option.value} value={option.label}>
                    {option.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
        </Box>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 2,
            px: 2,
            pb: 2,
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
              options={[{ label: "All Units", value: "ALL" }, ...(units || [])]}
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
              renderInput={(params) => <TextField {...params} size="small" />}
            />
          </FormControl>
        </Box>

        <Box sx={{ minHeight: "auto", width: "100%" }}>
          <DataGrid
            rows={data}
            columns={columns}
            getRowId={(row) => row.srNo}
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
      <Dialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        maxWidth={jobData.length > 0 ? "md" : "sm"}
        fullWidth
      >
        <DialogTitle
          sx={{
            textAlign: "center",
            fontWeight: 700,
            color: "#1a202c",
            borderBottom: "1px solid #e2e8f0",
            pb: 1.5,
            fontSize: "1.25rem",
          }}
        >
          Job Details
        </DialogTitle>
        <DialogContent>
          {jobLoading ? (
            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                py: 6,
              }}
            >
              <CircularProgress />
              <Typography
                variant="body2"
                sx={{ mt: 2, color: "text.secondary" }}
              >
                Loading job details...
              </Typography>
            </Box>
          ) : jobData && jobData.length > 0 ? (
            <Box
              sx={{
                display: "flex",
                flexDirection: "column",
                gap: 2,
                mt: 2,
              }}
            >
              {jobData.map((job, idx) => (
                <Box
                  key={idx}
                  sx={{
                    background: "#fff",
                    borderRadius: 2,
                    border: "1px solid #e2e8f0",
                    boxShadow: "0px 2px 10px rgba(0,0,0,0.08)",
                    overflow: "hidden",
                  }}
                >
                  <Box
                    sx={{
                      background: "#f7fafc",
                      px: 2,
                      py: 1,
                      fontWeight: 600,
                      fontSize: "0.9rem",
                      color: "#2d3748",
                      display: "flex",
                      justifyContent: "space-between",
                    }}
                  >
                    <span>Job No: {job.jobNo}</span>
                    <span>Serial: {job.srNo}</span>
                  </Box>

                  <Grid container>
                    {[
                      { label: "From Outer No", value: job.fromNoOuter },
                      { label: "To Outer No", value: job.toNoOuter },
                      { label: "From Master No", value: job.fromNoMaster },
                      { label: "To Master No", value: job.toNoMaster },
                      { label: "Quantity", value: job.qty },
                      {
                        label: "Last Outer Printed",
                        value: job.lastOuterPrinted,
                      },
                      {
                        label: "Last Master Printed",
                        value: job.lastMasterPrinted,
                      },
                      {
                        label: "Created On",
                        value: new Date(job.createdOn).toLocaleString(),
                      },
                    ].map((item, i) => (
                      <Grid
                        key={i}
                        size={{ xs: 12, md: 3 }}
                        sx={{
                          borderBottom: i < 6 ? "1px solid #edf2f7" : "none",
                          borderRight:
                            i % 2 === 0 ? "1px solid #edf2f7" : "none",
                          p: 1,
                        }}
                      >
                        <Typography
                          variant="caption"
                          sx={{
                            opacity: 0.6,
                            fontSize: "0.8rem",
                            fontWeight: 600,
                          }}
                        >
                          {item.label}
                        </Typography>
                        <Typography
                          variant="body2"
                          sx={{ fontWeight: 500, color: "#2d3748" }}
                        >
                          {item.value}
                        </Typography>
                      </Grid>
                    ))}
                  </Grid>
                </Box>
              ))}
            </Box>
          ) : (
            <Typography variant="body2" sx={{ textAlign: "center", py: 4 }}>
              No job details found.
            </Typography>
          )}
        </DialogContent>

        <DialogActions>
          <Button
            onClick={() => setDialogOpen(false)}
            variant="contained"
            sx={{
              backgroundColor: "#B71C1C",
              color: "#fff",
              fontWeight: 600,
              textTransform: "none",
              borderRadius: "8px",
              px: 3,
              py: 0.75,
              mr: 2,
              "&:hover": { backgroundColor: "#C62828" },
            }}
          >
            Close
          </Button>
        </DialogActions>
      </Dialog>
    </React.Fragment>
  );
};
