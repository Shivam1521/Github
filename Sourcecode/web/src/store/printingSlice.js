import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import PrintingService from "../services/PrintingService";

const service = new PrintingService();
export const serialUpload = createAsyncThunk(
  "printing/upload",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.SerialUploadExcel(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  }
);

const printingSlice = createSlice({
  name: "printing",
  initialState: {
    loading: false,
    uploadError: null,
    uploadSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.uploadError = null;
      state.uploadSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(serialUpload.pending, (state, action) => {
        state.loading = true;
        state.uploadError = null;
      })
      .addCase(serialUpload.fulfilled, (state, action) => {
        state.loading = false;
        state.uploadSuccess = action.payload;
      })
      .addCase(serialUpload.rejected, (state, action) => {
        state.loading = false;
        state.uploadError = action.payload;
      });
  },
});
export const { reset } = printingSlice.actions;
export default printingSlice.reducer;
