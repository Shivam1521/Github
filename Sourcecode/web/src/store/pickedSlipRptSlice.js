import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import PickedSlipRptService from "../services/PickedSlipRptService";

const service = new PickedSlipRptService();

export const fetchPickedSlipRptData = createAsyncThunk(
  "report/pickedSlip",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.fetchPickedSlipRpt(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch data");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

const pickedSlipRptSlice = createSlice({
  name: "pickedSlip",
  initialState: {
    data: [],
    totalRecords: 0,
    loading: false,
    error: null,
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPickedSlipRptData.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.data = [];
      })
      .addCase(fetchPickedSlipRptData.fulfilled, (state, action) => {
        state.loading = false;
        state.data = action.payload || [];
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchPickedSlipRptData.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.data = [];
        state.totalRecords = 0;
      });
  },
});
export default pickedSlipRptSlice.reducer;
