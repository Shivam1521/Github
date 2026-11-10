import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import PickSlipStatusService from "../services/PickSlipStatusService";

const service = new PickSlipStatusService();

export const fetchPickSlipData = createAsyncThunk(
  "report/pickSlip",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.fetchPickSlipData(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch data");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

const pickSlipStatusSlice = createSlice({
  name: "pickSlip",
  initialState: {
    data: [],
    loading: false,
    error: null,
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchPickSlipData.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.data = [];
      })
      .addCase(fetchPickSlipData.fulfilled, (state, action) => {
        state.loading = false;
        state.data = action.payload || [];
      })
      .addCase(fetchPickSlipData.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.data = [];
      });
  },
});
export default pickSlipStatusSlice.reducer;
