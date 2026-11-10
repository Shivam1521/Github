import React, { useState, useEffect } from "react";
import {
  Modal,
  Fade,
  Backdrop,
  Box,
  Typography,
  Grid,
  TextField,
  Button,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Autocomplete,
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import {
  fetchListHeaders,
  reset,
  saveListDetail,
} from "../../store/listMasterSlice";
import toast from "react-hot-toast";
import { fetchUnits } from "../../store/commonSlice";
import unitAccessHook from "../../components/UnitAccessHook";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 400,
  bgcolor: "background.paper",
  borderRadius: "8px",
  boxShadow: 24,
  p: 4,
};

const AddListModal = ({
  open,
  onClose,
  initialData,
  isEditMode,
  fetchListData,
}) => {
  const [formData, setFormData] = useState({
    listDetailId: "",
    listMasterId: "",
    listDetailName: "",
    unitId: "",
  });

  const dispatch = useDispatch();
  const [errors, setErrors] = useState({});
  const {
    submitLoading,
    createListDetailError,
    createListDetailSuccess,
    listHeaders,
  } = useSelector((state) => state.listMaster);
  const { units } = useSelector((state) => state.lists);
  const { isSuperAdmin, unitId } = unitAccessHook();

  useEffect(() => {
    dispatch(fetchListHeaders());
    dispatch(fetchUnits());
  }, []);

  useEffect(() => {
    if (createListDetailSuccess) {
      toast.success(createListDetailSuccess);
      fetchListData();
      onClose();
      dispatch(reset());
    }
    if (createListDetailError) {
      toast.error(createListDetailError);
      dispatch(reset());
    }
    return () => {};
  }, [createListDetailSuccess, createListDetailError]);

  useEffect(() => {
    if (isEditMode && initialData) {
      setFormData({
        listDetailId: initialData.listDetailId || "",
        listMasterId: initialData.listMasterId || "",
        listDetailName: initialData.listDetailName || "",
        unitId: initialData.unitId || "",
      });
    } else if (open && !isEditMode) {
      resetForm();
    }
  }, [initialData, isEditMode, open]);
  useEffect(() => {
    if (open && !isEditMode) {
      setFormData((prev) => ({
        ...prev,
        unitId: String(unitId),
      }));
    }
  }, [open, isEditMode]);
const validate = () => {
  const newErrors = {};

  // List Item
  if (!formData.listMasterId) {
    newErrors.listMasterId = "List Item is required.";
  }

  // List Detail Name
  if (!formData.listDetailName?.trim()) {
    newErrors.listDetailName = "List Detail Name is required.";
  } else if (
    !/^[a-zA-Z0-9\s.'()/_&-]{2,100}$/.test(
      formData.listDetailName.trim()
    )
  ) {
    newErrors.listDetailName =
      "List Detail Name contains invalid characters.";
  }

  // Unit - required only for Line
  if (formData.listMasterId === 4 && !formData.unitId) {
    newErrors.unitId = "Unit is required for Line.";
  }

  setErrors(newErrors);

  return Object.keys(newErrors).length === 0;
};

  // const validate = () => {
  //   const newErrors = {};

  //   if (!formData.listMasterId) {
  //     newErrors.listMasterId = "List Item is required.";
  //   }

  //   if (!formData.listDetailName || formData.listDetailName.trim() === "") {
  //     newErrors.listDetailName = "List Detail Name is required.";
  //   }
  //   if (formData.listMasterId === 4 && !formData.unitId) {
  //     newErrors.unitId = "Unit is required for Line";
  //   }
  //   setErrors(newErrors);
  //   return Object.keys(newErrors).length === 0;
  // };

  const handleChange = (field) => (event) => {
    setFormData({ ...formData, [field]: event.target.value });
    setErrors({ ...errors, [field]: "" });
  };
  const handleSave = () => {
    if (validate()) {
      const payload = {
        ListDetailId: isEditMode ? initialData.listDetailId : 0,
        ListMasterId: formData.listMasterId,
        ListDetailName: formData.listDetailName,
        UnitId: formData.unitId || 0,
        IsActive: isEditMode ? initialData.isActive : true,
      };
      dispatch(saveListDetail(payload));
    }
  };
  const resetForm = () => {
    setFormData({
      listDetailId: "",
      listMasterId: "",
      listDetailName: "",
      unitId: "",
      isActive: "",
    });
    setErrors({});
  };

  const handleModalClose = () => {
    resetForm();
    onClose();
  };

  return (
    <Modal
      open={open}
      onClose={handleModalClose}
      closeAfterTransition
      slots={{ backdrop: Backdrop }}
      slotProps={{ backdrop: { timeout: 500 } }}
    >
      <Fade in={open}>
        <Box sx={style}>
          <Typography variant="h6" mb={2}>
            {isEditMode ? "Update List" : "Add New List"}
          </Typography>

          <Grid container spacing={2} className="d-flex flex-column">
            <Grid size={{ md: 12, xs: 12 }}>
              <FormControl
                fullWidth
                size="small"
                error={!!errors.listMasterId}
                disabled={isEditMode}
              >
                <Autocomplete
                  options={listHeaders || []}
                  getOptionLabel={(option) => option.listName || ""}
                  value={
                    listHeaders.find(
                      (item) => item.listId === formData.listMasterId,
                    ) || null
                  }
                  onChange={(_, newValue) => {
                    handleChange("listMasterId")({
                      target: { value: newValue ? newValue.listId : "" },
                    });
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="List Item"
                      size="small"
                      error={!!errors.listMasterId}
                      helperText={errors.listMasterId}
                    />
                  )}
                  disabled={isEditMode}
                />
              </FormControl>
            </Grid>
            {formData.listMasterId === 4 && (
              <Grid size={{ md: 12, xs: 12 }}>
                <FormControl
                  fullWidth
                  size="small"
                  error={!!errors.unitId}
                  disabled={isEditMode}
                >
                  <Autocomplete
                    options={units || []}
                    disabled={!isSuperAdmin}
                    getOptionLabel={(option) => option.label || ""}
                    value={
                      units?.find(
                        (item) => item.value === String(formData.unitId),
                      ) || null
                    }
                    onChange={(_, newValue) => {
                      handleChange("unitId")({
                        target: { value: newValue?.value || "" },
                      });
                    }}
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        label="Select Unit"
                        size="small"
                        error={!!errors.unitId}
                        helperText={errors.unitId}
                      />
                    )}
                  />
                </FormControl>
              </Grid>
            )}

            <Grid size={{ md: 12, xs: 12 }}>
              <TextField
                fullWidth
                label="List Detail Name"
                size="small"
                value={formData.listDetailName}
                onChange={handleChange("listDetailName")}
                error={!!errors.listDetailName}
                helperText={errors.listDetailName}
              />
            </Grid>
          </Grid>

          <Grid size={{ md: 12 }} sx={{ mt: 2 }}>
            <Box display="flex" justifyContent="space-between" gap={2}>
              <Button
                fullWidth
                variant="contained"
                onClick={handleSave}
                disabled={submitLoading}
              >
                {submitLoading ? <CircularProgress size={20} /> : "Save"}
              </Button>
              <Button
                fullWidth
                variant="contained"
                color="error"
                onClick={handleModalClose}
              >
                Close
              </Button>
            </Box>
          </Grid>
        </Box>
      </Fade>
    </Modal>
  );
};

export default AddListModal;
