import React, { useEffect, useState } from "react";
import { Box, Button, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import CancelIcon from "@mui/icons-material/Cancel";
import EditIcon from "@mui/icons-material/Edit";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import { useDispatch, useSelector } from "react-redux";
import Restricted from "../../components/Restricted";
import { GridToolbar } from "@mui/x-data-grid/internals";
import { fetchListDetails } from "../../store/listMasterSlice";
import AddListModal from "./AddListModal";
import unitAccessHook from "../../components/UnitAccessHook";
import { exportListDataToExcel } from "../../helper/excelExport";
import ListMasterService from "../../services/ListMasterService";
import toast from "react-hot-toast";

export const ListMaster = () => {
  const { unitId } = unitAccessHook();
  const [open, setOpen] = useState(false);
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "CreateDate",
    sortOrder: "DESC",
    unitId: unitId || null,
  });
  const dispatch = useDispatch();
  const { datasource, totalRecords, loading, error } = useSelector(
    (state) => state.listMaster,
  );

  const [selectedRow, setSelectedRow] = useState({});
  const [isEditMode, setIsEditMode] = useState(false);
  const [isExporting, setIsExporting] = useState(false);
  const columns = [
    {
      field: "listName",
      headerName: "List Name",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "listDetailName",
      headerName: "List Detail Name",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "isActive",
      headerName: "Active",
      flex: 1,
      headerAlign: "center",
      align: "center",
      sortable: false,
      renderCell: (params) => (
        <input type="checkbox" checked={params.value} readOnly />
      ),
    },
    {
      field: "actions",
      headerName: "Action",
      flex: 1,
      sortable: false,
      headerAlign: "center",
      align: "center",
      renderCell: (params) => (
        <>
          <Restricted path="masters/employee-master" type="isEdit">
            <span style={{ color: "green", cursor: "pointer" }}>
              <EditIcon
                sx={{ mt: 1.5 }}
                onClick={() => {
                  setOpen(true);
                  setSelectedRow(params.row);
                  setIsEditMode(true);
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
        pageSize: pageSize,
      };
      dispatch(fetchListDetails(newRequest));
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
      dispatch(fetchListDetails(updatedRequest));
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

  const handleExportToExcel = async () => {
    try {
      setIsExporting(true);
      const listMasterService = new ListMasterService();

      // Create a request to fetch all records with a large page size
      const exportRequest = {
        pageNo: 1,
        searchValue: tableRequest.searchValue,
        pageSize: 10000, // Large page size to get all records
        sortColumn: tableRequest.sortColumn,
        sortOrder: tableRequest.sortOrder,
        unitId: String(unitId),
      };

      const response = await listMasterService.GetListDetails(exportRequest);

      if (response && response.responseData && response.responseData.length > 0) {
        const timestamp = new Date().toISOString().slice(0, 10);
        exportListDataToExcel(response.responseData, `list_data_${timestamp}.xlsx`);
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
      <Box
        sx={{
          display: "flex",
          gap: 1.5,
        }}
      >
        <Typography
          variant="h5"
          sx={{
            fontWeight: 600,
            color: "#333",
            letterSpacing: "0.5px",
            transition: "color 0.3s",
          }}
        >
          List Master
        </Typography>
      </Box>
      <hr />
      <Box sx={{ display: "flex", gap: 2, mb: 2 }}>
        <Restricted path="masters/employee-master" type="isAdd">
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
            onClick={() => {
              setOpen(true);
              setIsEditMode(false);
            }}
          >
            + Add
          </Button>
        </Restricted>
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
            display: "flex",
            gap: 1,
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
          <FileDownloadIcon sx={{ fontSize: "20px" }} />
          {isExporting ? "Exporting..." : "Export to Excel"}
        </Button>
      </Box>
      <Box sx={{ minHeight: "auto", width: "100%" }}>
        <DataGrid
          rows={datasource}
          getRowId={(row) => row.listDetailId}
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

      <AddListModal
        open={open}
        onClose={() => {
          setOpen(false);
        }}
        initialData={selectedRow}
        isEditMode={isEditMode}
        fetchListData={() =>
          dispatch(
            fetchListDetails({ ...tableRequest, pageNo: page + 1, pageSize }),
          )
        }
      />
    </React.Fragment>
  );
};
