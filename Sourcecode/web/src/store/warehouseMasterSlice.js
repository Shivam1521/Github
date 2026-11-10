import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import WarehouseMasterService from "../services/WarehouseMasterService";

const service = new WarehouseMasterService();
export const savePlantWarehouse = createAsyncThunk(
  "plantWarehouse/savePlantWarehouse",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.CreateUpdateUser(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

export const fetchPlantWarehouses = createAsyncThunk(
  "plantWarehouse/fetchPlantWarehouses",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetPlantWarehouses(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch users");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

const plantWarehouseSlice = createSlice({
  name: "plantWarehouse",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    submitLoading: false,
    createPlantWarehouseError: null,
    createPlantWarehouseSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createPlantWarehouseError = null;
      state.createPlantWarehouseSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPlantWarehouses.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPlantWarehouses.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchPlantWarehouses.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(savePlantWarehouse.pending, (state, action) => {
        state.submitLoading = true;
        state.createPlantWarehouseError = null;
      })
      .addCase(savePlantWarehouse.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createPlantWarehouseSuccess = action.payload;
      })
      .addCase(savePlantWarehouse.rejected, (state, action) => {
        state.submitLoading = false;
        state.createPlantWarehouseError = action.payload;
      });
  },
});
export const { reset } = plantWarehouseSlice.actions;
export default plantWarehouseSlice.reducer;
