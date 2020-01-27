using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using SharedServiceLibrary;

namespace ActionChessClient
{
    internal class CBoardRecordPlayerControlPresenter: CBoardControlPresenterBase, IDisposable
    {
        private readonly BoardRecordPlayerControl _attachedRecordPlayer;

        private readonly Dictionary<Int32, Image> _pieces;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private CMainPageViewModel _mainPageViewModel;

        public CBoardRecordPlayerControlPresenter(
            CMainPageViewModel mainPageViewModel,
            CBoardRecordData boardRecord,
            BoardRecordPlayerControl attachedRecordPlayer,
            Boolean isAutoBoardRecordPlayer)
        {
            _pieces = new Dictionary<Int32, Image>();
            _cancellationTokenSource = new CancellationTokenSource();
            _mainPageViewModel = mainPageViewModel;
            _attachedRecordPlayer = attachedRecordPlayer;
            InitializeBoardRecorderView();
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(async () =>
            {
                await PlayRecord(boardRecord, _cancellationTokenSource.Token);
                if(isAutoBoardRecordPlayer)
                    _mainPageViewModel?.SetRandomBoardRecord();
            }));
        }

        #region InitializeBoardRecorderStaff

        private void InitializeBoardRecorderView()
        {
            InitializePiecesCollection();
            ShowPieces();
        }

        protected override void InitializePiecesCollection()
        {
            void InitializeSide(Int32 yPieceCoordinate, Int32 firstPieceId, EPieceColor sideColor, Boolean mirrorOverX)
            {
                Int32 xPieceCoordinate;
                for (xPieceCoordinate = 0; xPieceCoordinate < 8; xPieceCoordinate++)
                    _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Pawn, new CTile(xPieceCoordinate, yPieceCoordinate)));

                if (mirrorOverX)
                    --yPieceCoordinate;
                else
                    ++yPieceCoordinate;

                xPieceCoordinate = 0;

                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Rook, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Knight, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Bishop, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Queen, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.King, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Bishop, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId++, CreatePieceImage(sideColor, EPieceType.Knight, new CTile(xPieceCoordinate++, yPieceCoordinate)));
                _pieces.Add(firstPieceId, CreatePieceImage(sideColor, EPieceType.Rook, new CTile(xPieceCoordinate, yPieceCoordinate)));
            }

            InitializeSide(6, 1, EPieceColor.White, false);
            InitializeSide(1, 17, EPieceColor.Black, true);
        }

        protected override void ShowPieces()
        {
            foreach (Image piece in _pieces.Values)
            {
                Panel.SetZIndex(piece, (Int32)EZIndex.Piece);
                _attachedRecordPlayer.BoardCanvas.Children.Add(piece);
            }
        }

        private static Image CreatePieceImage(EPieceColor color, EPieceType type, CTile tile)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri($"/Images/{color}/{type}.png", UriKind.RelativeOrAbsolute)),
                Width = CPiece.PieceSize,
                Height = CPiece.PieceSize,
                Margin = new Thickness(tile.X * CPiece.PieceSize, tile.Y * CPiece.PieceSize, 0, 0)
            };
        }

        #endregion

        private async Task PlayRecord(CBoardRecordData boardRecord, CancellationToken token)
        {
            foreach (CSerializedRecordAtom serializedRecordAtom in boardRecord.Data)
            {
                if (token.IsCancellationRequested)
                    return;
                switch (serializedRecordAtom.Type)
                {
                    case EAtomType.Movement:
                        CMovementData movementData = Deserialize<CMovementData>(serializedRecordAtom);
                        Thickness from = new Thickness(movementData.From.X, movementData.From.Y, 0, 0);
                        Thickness to = new Thickness(movementData.To.X, movementData.To.Y, 0, 0);
                        _pieces[movementData.PieceId].BeginAnimation(FrameworkElement.MarginProperty,
                            new ThicknessAnimation
                            {
                                From = from,
                                To = to,
                                SpeedRatio = CPiece.PieceSize / from.Difference(to)
                            });
                        break;
                    case EAtomType.PieceRemoved:
                        CPieceRemovedData pieceRemovedData = Deserialize<CPieceRemovedData>(serializedRecordAtom);
                        _attachedRecordPlayer.BoardCanvas.Children.Remove(_pieces[pieceRemovedData.PieceId]);
                        _pieces.Remove(pieceRemovedData.PieceId);
                        break;
                    case EAtomType.Wait:
                        CWaitData waitData = Deserialize<CWaitData>(serializedRecordAtom);
                        try
                        {
                            await Task.Delay(waitData.WaitTimeSpan, token);
                        }
                        catch
                        {
                            return;
                        }
                        break;
                    case EAtomType.PawnTransformed:
                        CPawnTransformedData pawnTransformedData = Deserialize<CPawnTransformedData>(serializedRecordAtom);
                        _pieces[pawnTransformedData.PieceId].Source = new BitmapImage(new Uri(
                            $"/Images/{(pawnTransformedData.IsWhite ? EPieceColor.White : EPieceColor.Black)}/Queen.png",
                                UriKind.RelativeOrAbsolute));
                        break;
                    default:
                        throw new ArgumentException($"Unknown atom type: {serializedRecordAtom.Type}");
                }
            }
        }

        private static T Deserialize<T>(CSerializedRecordAtom serializedRecordAtom)
        {
            return JsonConvert.DeserializeObject<T>(serializedRecordAtom.SerializedAtom);
        }

        public void Dispose()
        {
            _mainPageViewModel = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
