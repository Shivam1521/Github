import React, { useEffect, useState } from "react";
import { Box, Typography, Paper, Button } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useDispatch, useSelector } from "react-redux";
import Restricted from "../../components/Restricted";
//import { fetchItems } from "../../store/itemMasterSlice";
import EditIcon from "@mui/icons-material/Edit";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import EditItemModal from "./EditItemModal";
import { GridToolbar } from "@mui/x-data-grid/internals";
import unitAccessHook from "../../components/UnitAccessHook";
import ItemMasterService from "../../services/ItemMasterService";
import toast from "react-hot-toast";
import { exportItemDataToExcel } from "../../helper/excelExport";

import DownloadIcon from "@mui/icons-material/Download";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import {
  fetchItems,
  uploadItemMapping,
  reset,
} from "../../store/itemMasterSlice";
import sampleFile from "../../assets/ItemUserMappingSample.xlsx?url";

export const ItemMaster = () => {
  const { unitId } = unitAccessHook();
  const [open, setOpen] = useState(false);
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "UpdatedOn",
    sortOrder: "DESC",
    unitId: unitId || null,
  });
  const dispatch = useDispatch();
  const {
  datasource,
  totalRecords,
  loading,
  error,
  uploadLoading,
} = useSelector((state) => state.itemMaster);
  const [selectedRow, setSelectedRow] = useState({});
  const [isExporting, setIsExporting] = useState(false);

  const columns = [
    {
      field: "id",
      headerName: "ID",
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
      field: "description",
      headerName: "Description",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "brand",
      headerName: "Brand",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "mfgLocation",
      headerName: "Mfg Location",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "class",
      headerName: "Class",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "masterCartonPackingSize",
      headerName: "Master Carton Packing",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "outerBoxPackingSize",
      headerName: "Outer Box Packing",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "actions",
      headerName: "Action",
      flex: 1,
      sortable: false,

      renderCell: (params) => (
        <>
          <Restricted path="masters/item-master" type="isEdit">
            <span style={{ color: "green", cursor: "pointer" }}>
              <EditIcon
                sx={{ mt: 1.5 }}
                onClick={() => {
                  setOpen(true);
                  setSelectedRow({ ...params.row });
                }}
              />
            </span>
          </Restricted>
        </>
      ),
    },
  ];

  useEffect(() => {
    const storedToken = sessionStorage.getItem("token");
    if (storedToken) {
      const newRequest = {
        ...tableRequest,
        unitId: String(unitId),
        pageNo: page + 1,
        pageSize,
      };
      dispatch(fetchItems(newRequest));
    } else {
      navigate("/login");
    }
  }, [page, pageSize, tableRequest, unitId]);

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
  const handleUpload = async (e) => {
 const file = e.target.files[0];

if (!file) return;

const allowed = [".xlsx", ".xls", ".csv"];

const ext = file.name.substring(file.name.lastIndexOf(".")).toLowerCase();

if (!allowed.includes(ext)) {
    toast.error("Only Excel or CSV files are allowed.");
    e.target.value = "";
    return;
}

  try {
    const message = await dispatch(uploadItemMapping(file)).unwrap();

    toast.success(message);

dispatch(reset());

    dispatch(
      fetchItems({
        ...tableRequest,
        pageNo: page + 1,
        pageSize,
      })
    );
  } catch (err) {
    toast.error(err);
  }

  e.target.value = "";
};

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
const downloadSample = () => {
  const link = document.createElement("a");
  link.href = sampleFile;
  link.download = "ItemUserMappingSample.xlsx";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};
  const handleExportToExcel = async () => {
    try {
      setIsExporting(true);
      const itemMasterService = new ItemMasterService();

      // Create a request to fetch all records with a large page size
      const exportRequest = {
        pageNo: 1,
        searchValue: tableRequest.searchValue,
        pageSize: 150000, // Large page size to get all records
        sortColumn: tableRequest.sortColumn,
        sortOrder: tableRequest.sortOrder,
        unitId: String(unitId),
      };

      const response = await itemMasterService.GetItems(exportRequest);

      if (response && response.responseData && response.responseData.length > 0) {
        const timestamp = new Date().toISOString().slice(0, 10);
        exportItemDataToExcel(response.responseData, `item_data_${timestamp}.xlsx`);
        toast.success(`Exported ${response.responseData.length} records to Excel`);
      } else {
        toast.error("No data available to export");
      }
    } catch (error) {
      console.error("Error exporting data:", error);
      toast.error("Failed to export data");
    } finally {
      setIsExporting(false);
    }
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
          Item Master
        </Typography>
      </Box>
      <hr />
    <Box
  sx={{
    display: "flex",
    alignItems: "center",
    gap: 2,
    mb: 2,
    flexWrap: "wrap",
  }}
>
  <Button
    disabled={isExporting}
    sx={{
      backgroundColor: "#27AE60",
      color: "#fff",
      borderRadius: "8px",
      border: "2px solid #27AE60",
      fontWeight: 600,
      textTransform: "none",
      transition: "all 0.3s ease",
      px: 2,
      "&:hover": {
        backgroundColor: "#1E8449",
        borderColor: "#fff",
      },
      "&:disabled": {
        backgroundColor: "#95A5A6",
        cursor: "not-allowed",
        opacity: 0.7,
      },
    }}
    onClick={handleExportToExcel}
  >
    <FileDownloadIcon sx={{ mr: 1 }} />
    {isExporting ? "Exporting..." : "Export to Excel"}
  </Button>

  <Button
    variant="contained"
    color="primary"
    startIcon={<DownloadIcon />}
    onClick={downloadSample}
    sx={{
      borderRadius: "8px",
      fontWeight: 600,
      textTransform: "none",
      px: 2,
    }}
  >
    Download Sample
  </Button>

  <Button
    variant="contained"
    color="secondary"
    component="label"
    startIcon={<UploadFileIcon />}
    disabled={uploadLoading}
    sx={{
      borderRadius: "8px",
      fontWeight: 600,
      textTransform: "none",
      px: 2,
    }}
  >
    {uploadLoading ? "Uploading..." : "Upload Mapping"}

    <input
      hidden
      type="file"
      accept=".xlsx,.xls,.csv"
      onChange={handleUpload}
    />
  </Button>
</Box>
      <Paper elevation={2} sx={{ mt: 3 }}>
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
                sortModel: [{ field: "UpdatedOn", sort: "desc" }],
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
      <EditItemModal
        open={open}
        onClose={() => {
          setOpen(false);
        }}
        initialData={selectedRow}
        fetchUserData={() =>
          dispatch(fetchItems({ ...tableRequest, pageNo: page + 1, pageSize }))
        }
      />
    </React.Fragment>
  );
};
