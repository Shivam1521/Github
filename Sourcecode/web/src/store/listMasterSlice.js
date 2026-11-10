import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import ListMasterService from "../services/ListMasterService";

const service = new ListMasterService();
export const saveListDetail = createAsyncThunk(
  "listDetail/saveListDetails",
  async (payload, { rejectWithValue }) => {
    try {
      const res = await service.AddUpdateList(payload);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Unknown error");
      }

      return res.responseMessage;
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

export const fetchListDetails = createAsyncThunk(
  "listDetail/fetchListDetails",
  async (request, { rejectWithValue }) => {
    try {
      const res = await service.GetListDetails(request);

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch lists");
      }

      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);
export const fetchListHeaders = createAsyncThunk(
  "listDetail/fetchListHeaders",
  async (_, { rejectWithValue }) => {
    try {
      const res = await service.GetListHeaders();

      if (res.responseCode !== "200") {
        return rejectWithValue(res.responseMessage || "Failed to fetch lists");
      }
      return res.responseData || [];
    } catch (err) {
      return rejectWithValue(err.message || "Something went wrong");
    }
  },
);

const listDetailSlice = createSlice({
  name: "listDetail",
  initialState: {
    datasource: [],
    totalRecords: 0,
    loading: false,
    error: null,
    listLoading: false,
    listHeaders: [],
    submitLoading: false,
    createListDetailError: null,
    createListDetailSuccess: "",
  },
  reducers: {
    reset: (state) => {
      state.createListDetailError = null;
      state.createListDetailSuccess = "";
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchListDetails.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchListDetails.fulfilled, (state, action) => {
        state.loading = false;
        state.datasource = action.payload;
        state.totalRecords = action.payload.length
          ? action.payload[0].totalRows
          : 0;
      })
      .addCase(fetchListDetails.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
        state.datasource = [];
        state.totalRecords = 0;
      })
      .addCase(fetchListHeaders.pending, (state) => {
        state.listLoading = true;
        state.error = null;
      })
      .addCase(fetchListHeaders.fulfilled, (state, action) => {
        state.listLoading = false;
        state.listHeaders = action.payload;
      })
      .addCase(fetchListHeaders.rejected, (state, action) => {
        state.listLoading = false;
        state.error = action.payload;
      })
      .addCase(saveListDetail.pending, (state, action) => {
        state.submitLoading = true;
        state.createListDetailError = null;
      })
      .addCase(saveListDetail.fulfilled, (state, action) => {
        state.submitLoading = false;
        state.createListDetailSuccess = action.payload;
      })
      .addCase(saveListDetail.rejected, (state, action) => {
        state.submitLoading = false;
        state.createListDetailError = action.payload;
      });
  },
});
export const { reset } = listDetailSlice.actions;
export default listDetailSlice.reducer;
