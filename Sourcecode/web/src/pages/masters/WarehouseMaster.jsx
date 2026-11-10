import React, { useEffect, useState } from "react";
import Paper from "@mui/material/Paper";
import { Box, Button, Typography } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import CancelIcon from "@mui/icons-material/Cancel";
import EditIcon from "@mui/icons-material/Edit";
import AddWarehouseModal from "./AddWarehouseModal";
import { useDispatch, useSelector } from "react-redux";
import { fetchPlantWarehouses } from "../../store/warehouseMasterSlice";

export const WarehouseMaster = () => {
  const [open, setOpen] = useState(false);
  const [pageSize, setPageSize] = useState(5);
  const [page, setPage] = useState(0);
  const [tableRequest, setTableRequest] = useState({
    pageNo: 1,
    searchValue: "",
    pageSize: 5,
    sortColumn: "CreateDate",
    sortOrder: "DESC",
    userId: sessionStorage.getItem("userId"),
  });
  const dispatch = useDispatch();
  const { datasource, totalRecords, loading, error } = useSelector(
    (state) => state.plantWarehouseMaster
  );
  const [selectedRow, setSelectedRow] = useState({});
  const [isEditMode, setIsEditMode] = useState(false);
  const columns = [
    {
      field: "code",
      headerName: "Code",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "locationName",
      headerName: "Location",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "unitName",
      headerName: "Unit",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "name",
      headerName: "Name",
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
    {
      field: "mobileNumber",
      headerName: "Mobile Number",
      flex: 1,
      headerAlign: "center",
      align: "center",
    },
    {
      field: "isPlant",
      headerName: "Type",
      flex: 1,
      headerAlign: "center",
      align: "center",
      renderCell: (params) => {
        return params.row?.isPlant ? "Plant" : "Warehouse";
      },
    },

    {
      field: "actions",
      headerName: "Action",
      headerAlign: "center",
      align: "center",
      flex: 1,
      sortable: false,
      renderCell: (params) => (
        <>
          <span
            style={{
              color: "green",
              cursor: "pointer",
            }}
          >
            <EditIcon
              sx={{ mt: 1.5 }}
              onClick={() => {
                setOpen(true);
                setIsEditMode(true);
                setSelectedRow(params.row);
              }}
            />
          </span>
        </>
      ),
    },
  ];

  useEffect(() => {
    const storedToken = sessionStorage.getItem("token");
    if (storedToken) {
      dispatch(
        fetchPlantWarehouses({
          pageNo: page + 1,
          pageSize: pageSize,
          sortColumn: "CreateDate",
          searchValue: "",
          sortOrder: "DESC",
          userId: sessionStorage.getItem("userId"),
        })
      );
    } else {
      navigate("/login");
    }
  }, [page, pageSize, dispatch]);
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
      dispatch(fetchUnits(updatedRequest));
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
          Plant / Warehouse Master
        </Typography>
      </Box>
      <hr />
      <Box>
        <Button
          sx={{
            backgroundColor: "#1E2733",
            color: "#fff",
            my: 2,
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
      </Box>
      <Box sx={{ minHeight: "auto", width: "100%" }}>
        <DataGrid
          rows={datasource}
          getRowId={(row) => row.plantWarehouseId}
          columns={columns}
          pagination
          paginationMode="server"
          sortingMode="server"
          initialState={{
            sorting: {
              sortModel: [{ field: "createDate", sort: "desc" }],
            },
          }}
          onSortModelChange={handleSortModelChange}
          rowCount={totalRecords}
          pageSizeOptions={[5, 10, 25, 100]}
          paginationModel={{ page, pageSize }}
          onPaginationModelChange={(model) => {
            setPage(model.page);
            setPageSize(model.pageSize);
          }}
          loading={loading}
          slots={{ noRowsOverlay: CustomNoRowsOverlay }}
          disableRowSelectionOnClick
          disableColumnSelector
          disableColumnMenu
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
      <AddWarehouseModal
        open={open}
        onClose={() => {
          setOpen(false);
        }}
        initialData={selectedRow}
        isEditMode={isEditMode}
        fetchPlantWarehouseData={() =>
          dispatch(
            fetchPlantWarehouses({
              ...tableRequest,
              pageNo: page + 1,
              pageSize,
            })
          )
        }
      />
    </React.Fragment>
  );
};
