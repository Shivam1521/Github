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
  FormControlLabel,
  Checkbox,
} from "@mui/material";
import { useDispatch, useSelector } from "react-redux";
import { fetchList, fetchUnits } from "../../store/commonSlice";
import { reset, savePlantWarehouse } from "../../store/warehouseMasterSlice";
import toast from "react-hot-toast";

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

const AddWarehouseModal = ({
  open,
  onClose,
  initialData,
  isEditMode,
  fetchPlantWarehouseData,
}) => {
  const [formData, setFormData] = useState({
    plantWarehouseId: "",
    unitId: "",
    locationId: "",
    name: "",
    code: "",
    email: "",
    mobileNumber: "",
    isPlant: "",
  });
  const dispatch = useDispatch();
  const [errors, setErrors] = useState({});
  const { units } = useSelector((state) => state.lists);
  const { Location } = useSelector((state) => state.lists.listData);

  const {
    submitLoading,
    createPlantWarehouseError,
    createPlantWarehouseSuccess,
  } = useSelector((state) => state.plantWarehouseMaster);
  const userId = sessionStorage.getItem("userId");

  useEffect(() => {
    dispatch(fetchList("Location"));
  }, []);

  useEffect(() => {
    if (createPlantWarehouseSuccess) {
      toast.success(createPlantWarehouseSuccess);
      fetchPlantWarehouseData();
      onClose();
      dispatch(reset());
    }
    if (createPlantWarehouseError) {
      toast.error(createPlantWarehouseError);
      dispatch(reset());
    }
    return () => {};
  }, [createPlantWarehouseSuccess, createPlantWarehouseError]);

  useEffect(() => {
    if (isEditMode && initialData) {
      setFormData({
        mobileNumber: initialData.mobileNumber || "",
        email: initialData.email || "",
        unitId: initialData.unitId || "",
        locationId: initialData.locationId || "",
        name: initialData.name || "",
        code: initialData.code || "",
        isPlant: initialData.isPlant || "",
      });
    } else if (open && !isEditMode) {
      resetForm();
    }
  }, [initialData, isEditMode, open]);

  useEffect(() => {
    if (formData.locationId) {
      dispatch(fetchUnits(formData.locationId));
    }
  }, [formData.locationId]);

  // const validate = () => {
  //   const newErrors = {};

  //   if (!formData.locationId) newErrors.locationId = "Location is required.";
  //   if (!formData.unitId) newErrors.unitId = "Unit is required.";

  //   if (!formData.name) newErrors.name = "Name is required.";
  //   if (!formData.code) newErrors.code = "Code is required.";

  //   if (!formData.email) newErrors.email = "Email is required.";
  //   else if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(formData.email))
  //     newErrors.email = "Enter a valid email address.";

  //   if (!formData.mobileNumber)
  //     newErrors.mobileNumber = "Mobile Number is required.";
  //   else if (!/^\d{10}$/.test(formData.mobileNumber))
  //     newErrors.mobileNumber = "Enter a valid 10-digit mobile number.";

  //   setErrors(newErrors);
  //   return Object.keys(newErrors).length === 0;
  // };

  const validate = () => {
  const newErrors = {};

  // Location
  if (!formData.locationId) {
    newErrors.locationId = "Location is required.";
  }

  // Unit
  if (!formData.unitId) {
    newErrors.unitId = "Unit is required.";
  }

  // Name
  if (!formData.name?.trim()) {
    newErrors.name = "Name is required.";
  } else if (!/^[a-zA-Z0-9\s.'()/_&-]{2,100}$/.test(formData.name.trim())) {
    newErrors.name = "Name contains invalid characters.";
  }

  // Code
  if (!formData.code?.trim()) {
    newErrors.code = "Code is required.";
  } else if (!/^[a-zA-Z0-9_-]{1,30}$/.test(formData.code.trim())) {
    newErrors.code =
      "Code can contain only letters, numbers, '_' and '-'.";
  }

  // Email
  if (!formData.email?.trim()) {
    newErrors.email = "Email is required.";
  } else if (
    !/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(
      formData.email.trim()
    )
  ) {
    newErrors.email = "Enter a valid email address.";
  }

  // Mobile Number
  if (!formData.mobileNumber?.trim()) {
    newErrors.mobileNumber = "Mobile Number is required.";
  } else if (!/^\d{10}$/.test(formData.mobileNumber.trim())) {
    newErrors.mobileNumber = "Enter a valid 10-digit mobile number.";
  }

  setErrors(newErrors);

  return Object.keys(newErrors).length === 0;
};

  const handleChange = (field) => (event) => {
    setFormData({ ...formData, [field]: event.target.value });
    setErrors({ ...errors, [field]: "" });
  };

  
  const handleSave = () => {
    if (validate()) {
     const payload = {
  plantWarehouseId: isEditMode ? initialData.plantWarehouseId : 0,
  name: formData.name.trim(),
  code: formData.code.trim(),
  email: formData.email.trim(),
  locationId: formData.locationId,
  mobileNumber: formData.mobileNumber.trim(),
  unitId: formData.unitId,
  isPlant: formData.isPlant,
  userId: userId,
};
      dispatch(savePlantWarehouse(payload));
    }
  };
  const resetForm = () => {
    setFormData({
      plantWarehouseId: "",
      unitId: "",
      locationId: "",
      name: "",
      code: "",
      email: "",
      mobileNumber: "",
      isPlant: "",
      userId: "",
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
            {isEditMode
              ? "Update Warehouse / Plant"
              : "Add New Warehouse / Plant"}
          </Typography>

          <Grid container spacing={2} className="d-flex flex-column">
            <Grid size={{ md: 6, xs: 6 }}>
              <FormControl fullWidth size="small" error={!!errors.locationId}>
                <InputLabel>Location</InputLabel>
                <Select
                  value={formData.locationId}
                  label="Location"
                  onChange={handleChange("locationId")}
                >
                  {Location?.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.locationId && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.locationId}
                  </Typography>
                )}
              </FormControl>
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <FormControl
                fullWidth
                size="small"
                disabled={!formData.locationId}
                error={!!errors.unitId}
              >
                <InputLabel>Unit</InputLabel>
                <Select
                  value={formData.unitId}
                  label="Unit"
                  onChange={handleChange("unitId")}
                >
                  {units.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.unitId && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.unitId}
                  </Typography>
                )}
              </FormControl>
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Name"
                size="small"
                value={formData.name}
                onChange={handleChange("name")}
                error={!!errors.name}
                helperText={errors.name}
              />
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Code"
                size="small"
                value={formData.code}
                onChange={handleChange("code")}
                error={!!errors.code}
                helperText={errors.code}
              />
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Email"
                size="small"
                value={formData.email}
                onChange={handleChange("email")}
                error={!!errors.email}
                helperText={errors.email}
              />
            </Grid>
            <Grid size={{ md: 6, xs: 6 }}>
              <TextField
                fullWidth
                label="Mobile Number"
                size="small"
                value={formData.mobileNumber}
                onChange={handleChange("mobileNumber")}
                error={!!errors.mobileNumber}
                helperText={errors.mobileNumber}
              />
            </Grid>

            <Grid size={{ md: 6, xs: 6 }}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.isPlant}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        isPlant: e.target.checked,
                      })
                    }
                  />
                }
                label="Plant"
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

export default AddWarehouseModal;
