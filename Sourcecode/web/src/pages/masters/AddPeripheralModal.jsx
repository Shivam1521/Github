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
import { fetchList, fetchUnits } from "../../store/commonSlice";
import toast from "react-hot-toast";
import {
  reset,
  savePeripheralDevice,
} from "../../store/peripheralDeviceMasterSlice";
import unitAccessHook from "../../components/UnitAccessHook";

const style = {
  position: "absolute",
  top: "50%",
  left: "50%",
  transform: "translate(-50%, -50%)",
  width: 500,
  bgcolor: "background.paper",
  borderRadius: "8px",
  boxShadow: 24,
  p: 4,
};

const AddPeripheralModal = ({
  open,
  onClose,
  initialData,
  isEditMode,
  fetchDeviceData,
}) => {
  const { isSuperAdmin, unitId } = unitAccessHook();
  const [formData, setFormData] = useState({
    lineNoId: "",
    deviceTypeId: "",
    ip: "",
    port: "",
    brand: "",
    unitId: unitId || "",
  });

  const dispatch = useDispatch();
  const [errors, setErrors] = useState({});
  const {
    [`Line No&unitId=${formData.unitId}`]: LineNo,
    ["Device Type"]: DeviceType,
  } = useSelector((state) => state.lists.listData);
  const { units } = useSelector((state) => state.lists);
  const {
    submitLoading,
    createPeripheralDeviceError,
    createPeripheralDeviceSuccess,
  } = useSelector((state) => state.peripheralDeviceMaster);

  useEffect(() => {
    dispatch(fetchList("Device Type"));
    dispatch(fetchUnits());
  }, []);
  useEffect(() => {
    if (formData.unitId) {
      dispatch(fetchList(`Line No&unitId=${formData.unitId}`));
    }
  }, [formData.unitId]);

  useEffect(() => {
    if (createPeripheralDeviceSuccess) {
      toast.success(createPeripheralDeviceSuccess);
      fetchDeviceData();
      onClose();
      dispatch(reset());
    }
    if (createPeripheralDeviceError) {
      toast.error(createPeripheralDeviceError);
      dispatch(reset());
    }
  }, [createPeripheralDeviceSuccess, createPeripheralDeviceError]);

  useEffect(() => {
    if (isEditMode && initialData) {
      
      setFormData({
        lineNoId: initialData.lineNoId || "",
        deviceTypeId: initialData.deviceTypeId || "",
        ip: initialData.ip || "",
        port: initialData.port || "",
        brand: initialData.brand || "",
        unitId: String(initialData.unitId || unitId || ""),
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
    // Brand
    if (formData.brand && !/^[a-zA-Z0-9\s.'"-]+$/.test(formData.brand)) {
       newErrors.brand =
        'Brand can contain only letters, numbers, spaces, \', " and -';
      }
    if (!formData.lineNoId) newErrors.lineNoId = "Line No is required.";
    if (!formData.deviceTypeId)
      newErrors.deviceTypeId = "Device Type is required.";
    if (!formData.unitId) newErrors.unitId = "Unit is required.";
    if (!formData.ip) {
      newErrors.ip = "IP Address is required.";
    } else {
      const ipRegex =
        /^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)){3}$/;
      if (!ipRegex.test(formData.ip)) {
        newErrors.ip = "Enter a valid IPv4 address (e.g., 192.168.0.1).";
      }
    }
    if (!formData.port) {
      newErrors.port = "Port is required.";
    } else {
      const portRegex =
        /^([1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$/;
      if (!portRegex.test(formData.port)) {
        newErrors.port = "Enter a valid port (1–65535).";
      }
    }
    if (!formData.brand) newErrors.brand = "Brand is required.";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

const handleChange = (field) => (event) => {
  const value = event.target.value;

  setFormData({ ...formData, [field]: value });

  // Sirf Brand validation
  if (field === "brand") {
    if (value && !/^[a-zA-Z0-9\s.'"-]+$/.test(value)) {
      setErrors({
        ...errors,
        brand: 'Brand can contain only letters, numbers, spaces, \', " and -',
      });
      return;
    }
  }

  setErrors({
    ...errors,
    [field]: "",
  });
};

  const handleSave = () => {
    if (validate()) {
      const payload = {
        deviceId: isEditMode ? initialData.deviceId : 0,
        deviceTypeId: formData.deviceTypeId,
        lineNoId: formData.lineNoId,
        brand: formData.brand,
        ip: formData.ip,
        port: formData.port,
        unitId: formData.unitId,
        isActive: isEditMode ? initialData.isActive : true,
      };
      dispatch(savePeripheralDevice(payload));
    }
  };

  const resetForm = () => {
    setFormData({
      deviceId: "",
      lineNoId: "",
      deviceTypeId: "",
      ip: "",
      port: "",
      brand: "",
      unitId: "",
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
              ? "Update Peripheral Device"
              : "Add New Peripheral Device"}
          </Typography>
          <Grid container spacing={2}>
            <Grid size={{ md: 12, xs: 12 }}>
              <FormControl fullWidth size="small" error={!!errors.unitId}>
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
                      label="Unit"
                      size="small"
                      error={!!errors.unitId}
                      helperText={errors.unitId}
                    />
                  )}
                />
              </FormControl>
            </Grid>
            <Grid size={{ md: 6, xs: 12 }}>
              <FormControl fullWidth size="small" error={!!errors.lineNoId}>
                <Autocomplete
                  options={LineNo || []}
                  getOptionLabel={(option) => option.label || ""}
                  value={
                    LineNo?.find(
                      (item) =>
                        String(item.value) === String(formData.lineNoId),
                    ) || null
                  }
                  onChange={(_, newValue) => {
                    handleChange("lineNoId")({
                      target: { value: newValue ? newValue.value : "" },
                    });
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Line No"
                      size="small"
                      error={!!errors.lineNoId}
                      helperText={errors.lineNoId}
                    />
                  )}
                />
              </FormControl>
            </Grid>

            {/* Device Type */}
            <Grid size={{ md: 6, xs: 12 }}>
              <FormControl fullWidth size="small" error={!!errors.deviceTypeId}>
                <InputLabel>Device Type</InputLabel>
                <Select
                  value={formData.deviceTypeId}
                  label="Device Type"
                  onChange={handleChange("deviceTypeId")}
                >
                  {DeviceType?.map((item) => (
                    <MenuItem key={item.value} value={item.value}>
                      {item.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.deviceTypeId && (
                  <Typography variant="caption" color="error" sx={{ ml: 2 }}>
                    {errors.deviceTypeId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            {/* IP */}
            <Grid size={{ md: 6, xs: 12 }}>
              <TextField
                fullWidth
                label="IP"
                size="small"
                value={formData.ip}
                onChange={handleChange("ip")}
                error={!!errors.ip}
                helperText={errors.ip}
              />
            </Grid>

            {/* Port */}
            <Grid size={{ md: 6, xs: 12 }}>
              <TextField
                fullWidth
                label="Port"
                size="small"
                value={formData.port}
                onChange={handleChange("port")}
                error={!!errors.port}
                helperText={errors.port}
              />
            </Grid>
            <Grid size={{ md: 12, xs: 12 }}>
              <TextField
                fullWidth
                label="Brand"
                size="small"
                value={formData.brand}
                onChange={handleChange("brand")}
                error={!!errors.brand}
                helperText={errors.brand}
              />
            </Grid>

            {/* Actions */}
            <Grid size={{ md: 12, xs: 12 }}>
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
          </Grid>
        </Box>
      </Fade>
    </Modal>
  );
};

export default AddPeripheralModal;
