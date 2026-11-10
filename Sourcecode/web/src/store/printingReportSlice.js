import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import PrintingRptService from "../services/PrintingRptService";

const service = new PrintingRptService();

export const fetchPrintingData = createAsyncThunk(
  "report/printing",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.fetchPrintingData(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch data");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);
export const fetchJobData = createAsyncThunk(
  "report/printingJob",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.fetchJobData(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch data");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);
const printingRptSlice = createSlice({
  name: "printing",
  initialState: {
    data: [],
    totalRecords: 0,
    loading: false,
    error: null,
    jobData: [],
    jobLoading: false,
    jobError: null,
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPrintingData.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.data = [];
      })
      .addCase(fetchPrintingData.fulfilled, (state, action) => {
        state.loading = false;
        state.data = action.payload || [];
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchPrintingData.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.data = [];
        state.totalRecords = 0;
      })
      .addCase(fetchJobData.pending, (state) => {
        state.jobLoading = true;
        state.jobError = null;
        state.jobData = [];
      })
      .addCase(fetchJobData.fulfilled, (state, action) => {
        state.jobLoading = false;
        state.jobData = action.payload || [];
      })
      .addCase(fetchJobData.rejected, (state, action) => {
        state.jobLoading = false;
        state.jobError = action.payload;
        state.jobData = [];
      });
  },
});
export default printingRptSlice.reducer;
