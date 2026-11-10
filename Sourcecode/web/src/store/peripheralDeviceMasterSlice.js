import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import PeripheralDeviceMasterService from "../services/PeripheralDeviceMasterService";

const service = new PeripheralDeviceMasterService();
export const savePeripheralDevice = createAsyncThunk(
  "peripheralDevice/savePeripheralDevice",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.CreateUpdatePeripheralDevice(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

export const fetchPeripheralDevices = createAsyncThunk(
  "peripheralDevice/fetchUsers",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetPeripheralDevices(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(
          res.responseMessage || "Failed to fetch devices",
        );
      }
      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

const peripheralDeviceSlice = createSlice({
  name: "peripheralDevice",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    submitLoading: false,
    createPeripheralDeviceError: null,
    createPeripheralDeviceSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createPeripheralDeviceError = null;
      state.createPeripheralDeviceSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPeripheralDevices.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPeripheralDevices.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchPeripheralDevices.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.datasource = [];
        state.totalRecords = 0;
      })
      .addCase(savePeripheralDevice.pending, (state, action) => {
        state.submitLoading = true;
        state.createPeripheralDeviceError = null;
      })
      .addCase(savePeripheralDevice.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createPeripheralDeviceSuccess = action.payload;
      })
      .addCase(savePeripheralDevice.rejected, (state, action) => {
        state.submitLoading = false;
        state.createPeripheralDeviceError = action.payload;
      });
  },
});
export const { reset } = peripheralDeviceSlice.actions;
export default peripheralDeviceSlice.reducer;
